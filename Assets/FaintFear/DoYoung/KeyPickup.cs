using UnityEngine;
using UnityEngine.InputSystem; // ★ 추가

namespace FaintFear
{
    public class KeyPickup : MonoBehaviour
    {
        public bool hasKey = false;
        private bool isNearKey = false;
        private GameObject targetKeyObj;
        private ActionUI actionUI;

        private void Start()
        {
            actionUI = FindObjectOfType<ActionUI>();
        }

        // Input System 방식
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return; // E키가 눌렸을 때만
            if (isNearKey && targetKeyObj != null && !hasKey)
            {
                PickupKey();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Key") && !hasKey)
            {
                isNearKey = true;
                targetKeyObj = other.gameObject;
                actionUI?.ShowAction("열쇠 줍기");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Key"))
            {
                isNearKey = false;
                targetKeyObj = null;
                actionUI?.HideAction();
            }
        }

        private void PickupKey()
        {
            hasKey = true;
            Debug.Log("열쇠를 획득했다!");
            actionUI?.HideAction();
            if (targetKeyObj != null)
                Destroy(targetKeyObj);
        }
    }
}

