using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 플레이어 시점 기준 Raycast로 상호작용 대상을 감지하고 처리한다
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float rayDistance = 2f;          // 상호작용 Ray 최대 거리
        [SerializeField] private Transform cameraRoot;            // Ray 발사 기준 카메라
        [SerializeField] private LayerMask targetLayer;           // 상호작용 대상 레이어

        [SerializeField] private GameObject crossHair;             // 중앙 크로스헤어
        [SerializeField] private GameObject playerCrossHair;       // 전체 크로스헤어 UI

        [SerializeField] private ActionUI actionUI;                // 상호작용 문구 UI

        private PlayerMove playerMove;                              // 플레이어 이동 제어
        private GameObject target;                                  // 현재 Ray에 맞은 오브젝트
        private IActionProvider currentAction;                      // 현재 상호작용 대상

        private bool isOnRay = false;                                // Ray 적중 여부
        private bool isWall = false;                                 // 벽 판정 여부

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            // PlayerMove 컴포넌트를 캐싱한다
            playerMove = GetComponent<PlayerMove>();

            // 카메라가 지정되지 않았다면 자식 Camera에서 탐색한다
            if (cameraRoot == null)
                cameraRoot = GetComponentInChildren<Camera>()?.transform;

            // 시작 시 크로스헤어는 비활성화한다
            if (crossHair != null)
                crossHair.SetActive(false);
        }

        private void OnEnable()
        {
            // 상호작용 입력 이벤트를 구독한다
            if (playerMove != null)
                playerMove.OnInteractEvent += Interact;
        }

        private void OnDisable()
        {
            // 상호작용 입력 이벤트를 해제한다
            if (playerMove != null)
                playerMove.OnInteractEvent -= Interact;
        }

        private void Update()
        {
            // UI가 열려 있다면 상호작용을 중단한다
            if (UIState.IsUIOpen)
            {
                playerCrossHair?.SetActive(false);
                actionUI?.HideAction();
                return; // UI 우선 처리이므로 이후 로직을 실행하지 않는다
            }

            // 플레이어 조작이 잠겨 있다면 상호작용을 중단한다
            if (!playerMove.enabled)
            {
                crossHair?.SetActive(false);
                actionUI?.HideAction();
                return; // 이동 불가 상태에서는 Ray를 쏘지 않는다
            }

            // 상호작용 Raycast를 실행한다
            ShootRay();
        }

        #endregion


        #region Custom Method

        // 카메라 전방으로 Ray를 발사해 상호작용 대상을 감지한다
        private void ShootRay()
        {
            playerCrossHair?.SetActive(true);

            Vector3 origin = cameraRoot.position;
            Vector3 direction = cameraRoot.forward;

            Debug.DrawRay(origin, direction * rayDistance, Color.green);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, targetLayer))
            {
                target = hit.transform.gameObject;
                isOnRay = true;

                // Ray에 맞은 대상이 벽이면 상호작용을 차단한다
                if (target.CompareTag("Wall"))
                {
                    isWall = true;
                    crossHair?.SetActive(false);
                    actionUI?.HideAction();
                    currentAction = null;
                    return; // 벽은 상호작용 대상이 아니므로 종료한다
                }

                isWall = false;
                crossHair?.SetActive(true);

                // IActionProvider 구현 여부를 확인한다
                IActionProvider action = target.GetComponentInParent<IActionProvider>();
                if (action != null)
                {
                    currentAction = action;
                    actionUI?.ShowAction(action.GetActionText()); // 대상이 제공하는 문구를 출력한다
                }
                else
                {
                    actionUI?.HideAction(); // 상호작용 불가 대상이면 문구를 숨긴다
                    currentAction = null;
                }
            }
            else
            {
                // Ray가 아무것도 맞추지 못했을 경우 상태를 초기화한다
                crossHair?.SetActive(false);
                actionUI?.HideAction();

                target = null;
                isOnRay = false;
                isWall = false;
                currentAction = null;
            }
        }

        // 상호작용 입력(E 키)이 들어왔을 때 호출된다
        private void Interact()
        {
            // 벽이 아니고 && Ray에 대상이 있으며 && 대상이 존재할 때만 처리한다
            if (!isWall && isOnRay && target != null)
            {
                Interactive interactive = target.GetComponentInParent<Interactive>();
                if (interactive != null)
                    interactive.Interaction(); // 실제 상호작용을 실행한다
            }
        }

        #endregion
    }
}
