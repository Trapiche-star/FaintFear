using UnityEngine;
using System.Collections;

namespace FaintFear
{
    public class DoorDocumentTrigger : LockedDoorBase, IActionProvider
    {
        [SerializeField] private Transform hinge;

        [Header("Locked Messages")]
        [SerializeField] private string[] lockedMessages;
        [SerializeField] private string defaultLockedMessage = "잠겨 있는 것 같다.";

        private int messageIndex = 0;

        // ⭐ 추가: Start에서 퍼즐 완료 상태 확인
        private void Start()
        {
            // 퍼즐이 이미 완료되었다면 잠금 해제
            if (DocumentPuzzleManager.Instance != null &&
                DocumentPuzzleManager.Instance.IsCompleted)
            {
                isLocked = false;
                Debug.Log($"[DoorDocumentTrigger] 퍼즐 완료 상태 복원: {gameObject.name}");
            }
        }

        public void SetUnlocked(bool unlocked)
        {
            isLocked = !unlocked;

            if (!isLocked)
            {
                RecordUnlockState();
                Debug.Log($"[DoorDocumentTrigger] 잠금 해제됨: {gameObject.name}");
            }
        }

        protected override bool CanUnlock()
        {
            return false;
        }

        protected override void ToggleDoor()
        {
            StartCoroutine(MoveDoorRoutine(isOpen ? 0f : -90f));
            isOpen = !isOpen;
        }

        private IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true;
            float elapsed = 0f;
            float duration = 1f;
            Quaternion startRot = hinge.localRotation;
            Quaternion targetRot = Quaternion.Euler(0, targetAngle, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                hinge.localRotation = Quaternion.Lerp(startRot, targetRot, elapsed / duration);
                yield return null;
            }

            hinge.localRotation = targetRot;
            isMoving = false;
        }

        protected override void ShowLockedMessage()
        {
            if (sequenceText == null) return;

            string msg = lockedMessages != null && lockedMessages.Length > 0
                ? lockedMessages[messageIndex]
                : defaultLockedMessage;

            messageIndex++;
            if (messageIndex >= lockedMessages.Length)
                messageIndex = 0;

            sequenceText.ShowMessage(string.IsNullOrWhiteSpace(msg) ? defaultLockedMessage : msg);
        }

        protected override void ApplyDoorRotation()
        {
            if (hinge != null)
                hinge.localRotation = Quaternion.Euler(0, isOpen ? -90f : 0, 0);
        }

        public string GetActionText()
        {
            return isOpen ? "[E] 문 닫기" : "[E] 문 열기";
        }
    }
}