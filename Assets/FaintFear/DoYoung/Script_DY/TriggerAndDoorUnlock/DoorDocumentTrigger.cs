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

        // 퍼즐 완료 신호로 잠금 해제
        public void SetUnlocked(bool unlocked)
        {
            isLocked = !unlocked;

            // ⭐ 잠금 해제 시 저장 (protected 메서드 호출)
            if (!isLocked)
            {
                RecordUnlockState();
            }
        }

        protected override bool CanUnlock()
        {
            // 이 문은 퍼즐 완료로만 열리므로 여기선 항상 false
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