using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 플레이어 상호작용 처리
    /// - Raycast로 상호작용 대상 감지
    /// - IActionProvider를 통해 ActionUI 문구 표시
    /// - E 키 입력 시 Interactive 실행
    /// </summary>
    public class PlayerInteraction_DY : MonoBehaviour
    {
        #region Variables

        // Raycast 최대 거리
        [SerializeField] private float rayDistance = 2f;

        // Raycast 발사 기준 (카메라)
        [SerializeField] private Transform cameraRoot;

        // 화면 중앙 크로스헤어
        [SerializeField] private GameObject crossHiair;

        // 상호작용 가능한 레이어
        [SerializeField] private LayerMask targetLayer;

        // 액션 문구 UI
        [SerializeField] private ActionUI actionUI;

        // 플레이어 이동 / 입력 이벤트용
        private PlayerMove playerMove;

        // Ray가 대상에 맞았는지 여부
        private bool isOnLay = false;

        // 현재 대상이 벽인지 여부
        private bool isWall = false;

        // Ray에 맞은 오브젝트
        private GameObject target;

        // 현재 바라보는 상호작용 대상
        private IActionProvider currentAction;

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            // PlayerMove 컴포넌트 가져오기
            playerMove = GetComponent<PlayerMove>();

            // 카메라 자동 탐색
            if (cameraRoot == null)
                cameraRoot = GetComponentInChildren<Camera>().transform;

            // 크로스헤어 자동 연결
            crossHiair = transform.GetChild(1).GetChild(0).gameObject;

            // ActionUI 자동 탐색 (씬에 하나라고 가정)
            if (actionUI == null)
                actionUI = FindFirstObjectByType<ActionUI>();

            // 시작 시 크로스헤어 비활성화
            if (crossHiair != null)
                crossHiair.SetActive(false);
        }

        private void OnEnable()
        {
            // E 키 입력 이벤트 구독
            if (playerMove != null)
                playerMove.OnInteractEvent += Interact;
        }

        private void OnDisable()
        {
            // E 키 입력 이벤트 해제
            if (playerMove != null)
                playerMove.OnInteractEvent -= Interact;
        }

        private void Update()
        {
            // 플레이어 조작이 잠겨 있으면 처리 중단
            if (!playerMove.enabled)
            {
                crossHiair?.SetActive(false);
                actionUI?.HideAction();
                return;
            }

            // 상호작용 Raycast 실행
            ShootRay();
        }

        #endregion

        #region Custom Method

        // 카메라 전방으로 Raycast를 쏴서 상호작용 대상 감지
        private void ShootRay()
        {
            Vector3 rayOrigin = cameraRoot.position;
            Vector3 rayDirection = cameraRoot.forward;

            // Scene 뷰 디버그용 Ray
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.green, 1f);

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, targetLayer))
            {
                // Ray가 무언가에 맞았을 때
                target = hit.transform.gameObject;
                isOnLay = true;

                // 벽일 경우 상호작용 차단
                if (target.CompareTag("Wall"))
                {
                    isWall = true;
                    crossHiair.SetActive(false);
                    actionUI?.HideAction();
                    currentAction = null;
                }
                else
                {
                    // 상호작용 가능한 대상
                    isWall = false;
                    crossHiair.SetActive(true);

                    // IActionProvider 구현 여부 확인
                    IActionProvider action = target.GetComponentInParent<IActionProvider>();
                    if (action != null)
                    {
                        // 대상이 제공하는 문구 표시
                        actionUI?.ShowAction(action.GetActionText());
                        currentAction = action;
                    }
                    else
                    {
                        // 상호작용 대상이 아니면 UI 숨김
                        actionUI?.HideAction();
                        currentAction = null;
                    }
                }
            }
            else
            {
                // 아무것도 맞지 않았을 때 초기화
                crossHiair.SetActive(false);
                actionUI?.HideAction();

                target = null;
                isOnLay = false;
                isWall = false;
                currentAction = null;
            }
        }

        // E 키 입력 시 호출
        private void Interact()
        {
            // 벽이 아니고 상호작용 대상이 있을 때만 실행
            if (!isWall && isOnLay && target != null)
            {
                Interactive interactive = target.GetComponentInParent<Interactive>();
                if (interactive != null)
                {
                    interactive.Interaction();
                }
            }
        }

        #endregion
    }
}
