using NavKeypad;
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

            // ⭐ 마우스 왼쪽 클릭으로 키패드 버튼 누르기
            if (Input.GetMouseButtonDown(0))
            {
                CheckKeypadButton();
            }
        }

        #endregion

        #region Custom Method

        private void OnPushStateChanged(bool isPushing)
        {
            if (isPushing)
            {
                crossHair?.SetActive(false);
                actionUI?.HideAction();
                currentPushingItem = previousPushItem;
            }
            else
            {
                currentPushingItem = null;
            }
        }

        /// <summary>
        /// ⭐ 키패드 버튼 클릭 체크
        /// </summary>
        private void CheckKeypadButton()
        {
            if (cameraRoot == null) return;

            Ray ray = new Ray(cameraRoot.position, cameraRoot.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance, targetLayer))
            {
                // KeypadButton 컴포넌트 체크
                KeypadButton keypadButton = hit.collider.GetComponent<KeypadButton>();
                if (keypadButton != null)
                {
                    keypadButton.PressButton();
                    return;
                }

                // 부모에서도 찾아보기
                keypadButton = hit.collider.GetComponentInParent<KeypadButton>();
                if (keypadButton != null)
                {
                    keypadButton.PressButton();
                }
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