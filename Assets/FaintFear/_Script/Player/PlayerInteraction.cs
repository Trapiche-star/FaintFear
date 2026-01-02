using UnityEngine;

namespace FaintFear
{
    public class PlayerInteraction : MonoBehaviour
    {
        #region Variables

        [Header("Ray Settings")]
        [SerializeField] private float rayDistance = 2f;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private LayerMask targetLayer;

        [Header("UI")]
        [SerializeField] private GameObject crossHair;
        [SerializeField] private GameObject playerCrossHair;
        [SerializeField] private ActionUI actionUI;

        private PlayerMove playerMove;
        private GameObject target;
        private IActionProvider currentAction;

        private PushItem previousPushItem;
        // ⭐ 현재 PushItem을 밀고 있는지 추적
        private PushItem currentPushingItem;

        private bool isOnRay = false;
        private bool isWall = false;

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            playerMove = GetComponent<PlayerMove>();

            if (cameraRoot == null)
                cameraRoot = GetComponentInChildren<Camera>()?.transform;

            if (crossHair != null)
                crossHair.SetActive(false);
        }

        private void OnEnable()
        {
            if (playerMove != null)
            {
                playerMove.OnInteractEvent += Interact;
                // ⭐ Push 이벤트 구독 (UI 숨김용)
                playerMove.OnPushEvent += OnPushStateChanged;
            }
        }

        private void OnDisable()
        {
            if (playerMove != null)
            {
                playerMove.OnInteractEvent -= Interact;
                playerMove.OnPushEvent -= OnPushStateChanged;
            }

            if (previousPushItem != null)
            {
                previousPushItem.DisablePushing();
                previousPushItem = null;
            }
        }

        private void Update()
        {
            if (playerMove == null || !playerMove.enabled)
            {
                crossHair?.SetActive(false);
                actionUI?.HideAction();

                if (previousPushItem != null)
                {
                    previousPushItem.DisablePushing();
                    previousPushItem = null;
                }

                return;
            }

            ShootRay();
        }

        #endregion

        #region Custom Method

        // ⭐ V키 입력 상태 변경 시 호출
        private void OnPushStateChanged(bool isPushing)
        {
            if (isPushing)
            {
                // ⭐ V키 누르는 순간 crossHair와 actionUI 숨김
                crossHair?.SetActive(false);
                actionUI?.HideAction();
                currentPushingItem = previousPushItem;
            }
            else
            {
                // ⭐ V키 뗐을 때 UI 복구 (게이지가 0이 아니면 유지)
                currentPushingItem = null;
            }
        }

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

                if (target.CompareTag("Wall"))
                {
                    isWall = true;
                    crossHair?.SetActive(false);
                    actionUI?.HideAction();
                    currentAction = null;

                    if (previousPushItem != null)
                    {
                        previousPushItem.DisablePushing();
                        previousPushItem = null;
                    }

                    return;
                }

                isWall = false;

                PushItem pushItem = target.GetComponentInParent<PushItem>();
                if (pushItem != null)
                {
                    // ⭐ 이미 움직인 오브젝트는 무시
                    if (pushItem.GetComponent<PushItem>() != null &&
                        pushItem.GetActionText() == "")
                    {
                        crossHair?.SetActive(false);
                        actionUI?.HideAction();
                        currentAction = null;

                        if (previousPushItem != null && previousPushItem != pushItem)
                        {
                            previousPushItem.DisablePushing();
                            previousPushItem = null;
                        }

                        return;
                    }

                    if (previousPushItem != pushItem)
                    {
                        if (previousPushItem != null)
                        {
                            previousPushItem.DisablePushing();
                        }

                        pushItem.EnablePushing();
                        previousPushItem = pushItem;
                    }

                    // ⭐ V키를 누르고 있지 않을 때만 UI 표시
                    if (currentPushingItem == null)
                    {
                        crossHair?.SetActive(true);
                    }
                }
                else
                {
                    if (previousPushItem != null)
                    {
                        previousPushItem.DisablePushing();
                        previousPushItem = null;
                    }

                    crossHair?.SetActive(true);
                }

                // ActionUI 처리
                IActionProvider action = target.GetComponentInParent<IActionProvider>();
                if (action != null)
                {
                    currentAction = action;
                    string actionText = action.GetActionText();

                    // ⭐ V키 누르는 중이 아닐 때만 텍스트 표시
                    if (currentPushingItem == null && !string.IsNullOrEmpty(actionText))
                    {
                        actionUI?.ShowAction(actionText);
                    }
                    else
                    {
                        actionUI?.HideAction();
                    }
                }
                else
                {
                    actionUI?.HideAction();
                    currentAction = null;
                }
            }
            else
            {
                crossHair?.SetActive(false);
                actionUI?.HideAction();

                if (previousPushItem != null)
                {
                    previousPushItem.DisablePushing();
                    previousPushItem = null;
                }

                target = null;
                isOnRay = false;
                isWall = false;
                currentAction = null;
            }
        }

        private void Interact()
        {
            if (!isWall && isOnRay && target != null)
            {
                Interactive interactive = target.GetComponentInParent<Interactive>();
                if (interactive != null)
                {
                    if (!(interactive is PushItem))
                    {
                        interactive.Interaction();
                    }
                }
            }
        }

        #endregion
    }
}