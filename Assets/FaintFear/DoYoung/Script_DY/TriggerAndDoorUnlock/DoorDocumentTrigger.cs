using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 문서 퍼즐 완료 여부에 따라 잠금이 해제되는 도어
    /// 씬 로드 시 퍼즐 상태를 조회하여 스스로 잠금 상태를 복원한다
    /// </summary>
    public class DoorDocumentTrigger : LockedDoorBase, IActionProvider
    {
        #region Variables

        [SerializeField] private Transform hinge; // 문 회전을 담당하는 힌지 트랜스폼

        [Header("Locked Messages")]
        [SerializeField] private string[] lockedMessages;           // 잠금 상태일 때 출력할 메시지 목록
        [SerializeField] private string defaultLockedMessage = "잠겨 있는 것 같다."; // 기본 잠금 메시지

        private int messageIndex = 0; // 메시지 순환 인덱스

        #endregion


        #region Unity Event Method

        // 씬 시작 시 퍼즐 완료 상태를 확인하여 잠금 여부를 복원한다
        private void Start()
        {
            if (DocumentPuzzleManager.Instance != null &&
                DocumentPuzzleManager.Instance.IsCompleted)
            {
                isLocked = false;
                Debug.Log($"[DoorDocumentTrigger] 퍼즐 완료 상태 복원: {gameObject.name}");
            }
        }

        #endregion


        #region Custom Method

        // 외부에서 잠금 상태를 직접 설정한다
        public void SetUnlocked(bool unlocked)
        {
            isLocked = !unlocked;

            if (!isLocked)
            {
                RecordUnlockState();
                Debug.Log($"[DoorDocumentTrigger] 잠금 해제됨: {gameObject.name}");
            }
        }

        // 잠금 해제 조건은 외부 퍼즐 로직에서만 처리하므로 내부 판정은 항상 false를 반환한다
        protected override bool CanUnlock()
        {
            return false;
        }

        // 문을 열거나 닫는 동작을 수행한다
        protected override void ToggleDoor()
        {
            StartCoroutine(MoveDoorRoutine(isOpen ? 0f : -90f));
            isOpen = !isOpen;
        }

        // 문 회전 애니메이션을 처리한다
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

        // 잠긴 상태에서 메시지를 출력한다
        protected override void ShowLockedMessage()
        {
            if (sequenceText == null) return; // 만약 텍스트 매니저가 없다면 출력하지 않는다

            string msg = lockedMessages != null && lockedMessages.Length > 0
                ? lockedMessages[messageIndex]
                : defaultLockedMessage;

            messageIndex++;
            if (messageIndex >= lockedMessages.Length)
                messageIndex = 0;

            sequenceText.ShowMessage(string.IsNullOrWhiteSpace(msg) ? defaultLockedMessage : msg);
        }

        // 현재 문 상태에 맞게 회전을 적용한다
        protected override void ApplyDoorRotation()
        {
            if (hinge != null)
                hinge.localRotation = Quaternion.Euler(0, isOpen ? -90f : 0, 0);
        }

        // 상호작용 UI 문구를 반환한다
        public string GetActionText()
        {
            return isOpen ? "[E] 문 닫기" : "[E] 문 열기";
        }

        #endregion
    }
}
