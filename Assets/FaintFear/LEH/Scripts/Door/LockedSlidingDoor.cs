using UnityEngine;

namespace NavKeypad
{
    public class LockedSlidingDoor : MonoBehaviour
    {
        [SerializeField] private SlidingDoor slidingDoor;
        [SerializeField] private GameObject interactUI; // "E 눌러 문열기"

        private bool isUnlocked = false;
        private bool mouseOver = false;

        void Start()
        {
            if (interactUI != null)
                interactUI.SetActive(false);
        }

        void Update()
        {
            if (!isUnlocked) return;
            if (!mouseOver) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                slidingDoor.OpenDoor();
                if (interactUI != null)
                    interactUI.SetActive(false);
            }
        }

        // 🔓 키패드 성공 시 호출
        public void UnlockDoor()
        {
            isUnlocked = true;
        }

        void OnMouseEnter()
        {
            if (!isUnlocked) return;
            mouseOver = true;
            if (interactUI != null)
                interactUI.SetActive(true);
        }

        void OnMouseExit()
        {
            mouseOver = false;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }
}
