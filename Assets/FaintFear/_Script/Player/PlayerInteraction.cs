using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 플레이어 시점 기준 Raycast로 상호작용 대상을 감지하고 처리한다
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        #region Variables

        [Header("Ray Settings")]
        [SerializeField] private float rayDistance = 2f;      // 상호작용 Ray 최대 거리
        [SerializeField] private Transform cameraRoot;        // Ray 발사 기준 카메라
        [SerializeField] private LayerMask targetLayer;       // 상호작용 대상 레이어

        [Header("UI")]
        [SerializeField] private GameObject crossHair;        // 중앙 크로스헤어
        [SerializeField] private GameObject playerCrossHair;  // 전체 크로스헤어 UI
        [SerializeField] private ActionUI actionUI;           // 상호작용 문구 UI

        private PlayerMove playerMove;                         // 플레이어 이동 제어
        private GameObject target;                             // 현재 Ray에 맞은 오브젝트
        private IActionProvider currentAction;                 // 현재 상호작용 대상

        private bool isOnRay = false;                           // Ray 적중 여부
        private bool isWall = false;                            // 벽 판정 여부

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            playerMove = GetComponent<PlayerMove>();
            // 플레이어 이동 컴포넌트를 캐싱한다

            if (cameraRoot == null)
                cameraRoot = GetComponentInChildren<Camera>()?.transform;
            // 만약 [카메라가 지정되지 않았다면] [자식 Camera를 기준으로 설정한다]

            if (crossHair != null)
                crossHair.SetActive(false);
            // 시작 시 크로스헤어를 비활성화한다
        }

        private void OnEnable()
        {
            if (playerMove != null)
                playerMove.OnInteractEvent += Interact;
            // 상호작용 입력 이벤트를 구독한다
        }

        private void OnDisable()
        {
            if (playerMove != null)
                playerMove.OnInteractEvent -= Interact;
            // 상호작용 입력 이벤트를 해제한다
        }

        private void Update()
        {
            if (playerMove == null || !playerMove.enabled)
            {
                crossHair?.SetActive(false);
                actionUI?.HideAction();
                return; // 만약 [플레이어 이동이 불가능한 상태라면] [상호작용을 중단한다]
            }

            ShootRay();
            // 이동 가능한 상태일 때만 상호작용 Raycast를 실행한다
        }

        #endregion


        #region Custom Method

        // 카메라 전방으로 Ray를 발사해 상호작용 대상을 감지한다
        private void ShootRay()
        {
            playerCrossHair?.SetActive(true);
            // 전체 크로스헤어 UI를 활성화한다

            Vector3 origin = cameraRoot.position;
            Vector3 direction = cameraRoot.forward;

            Debug.DrawRay(origin, direction * rayDistance, Color.green);
            // Scene 뷰에서 Ray 방향을 시각화한다

            if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, targetLayer))
            {
                target = hit.transform.gameObject;
                isOnRay = true;
                // Ray가 대상에 적중했음을 기록한다

                if (target.CompareTag("Wall"))
                {
                    isWall = true;
                    crossHair?.SetActive(false);
                    actionUI?.HideAction();
                    currentAction = null;
                    return; // 만약 [벽이라면] [상호작용을 차단한다]
                }

                isWall = false;
                crossHair?.SetActive(true);
                // 상호작용 가능한 대상이므로 크로스헤어를 표시한다

                IActionProvider action = target.GetComponentInParent<IActionProvider>();
                if (action != null)
                {
                    currentAction = action;
                    actionUI?.ShowAction(action.GetActionText());
                    // 만약 [상호작용 인터페이스가 있다면] [액션 문구를 출력한다]
                }
                else
                {
                    actionUI?.HideAction();
                    currentAction = null;
                    // 상호작용 불가 대상이면 UI를 숨긴다
                }
            }
            else
            {
                crossHair?.SetActive(false);
                actionUI?.HideAction();

                target = null;
                isOnRay = false;
                isWall = false;
                currentAction = null;
                // Ray가 아무것도 맞추지 못했을 경우 상태를 초기화한다
            }
        }

        // 상호작용 입력(E 키)이 들어왔을 때 호출된다
        private void Interact()
        {
            if (!isWall && isOnRay && target != null)
            {
                Interactive interactive = target.GetComponentInParent<Interactive>();
                if (interactive != null)
                    interactive.Interaction();
                // 만약 [상호작용 대상이 존재한다면] [상호작용을 실행한다]
            }
        }

        #endregion
    }
}
