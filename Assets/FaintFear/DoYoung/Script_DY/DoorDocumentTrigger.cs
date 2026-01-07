using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 문서 퍼즐 조건으로 열리는 상호작용 도어
    /// 기본은 잠김 상태이며 퍼즐 완료 신호를 받으면 개방 가능 상태로 전환된다.
    /// </summary>
    public class DoorDocumentTrigger : Interactive, IActionProvider
    {
        #region Variables

        private Transform hinge;               // 문 회전을 담당하는 힌지 트랜스폼
        private bool isMoving = false;         // 문 애니메이션 진행 여부
        private bool isOpen = false;           // 문 개방 상태

        [Header("Door State")]
        [SerializeField] private bool interactionEnabled = true; // 도어 상호작용 가능 여부
        [SerializeField] private bool isLocked = true;           // 문 잠금 상태 (퍼즐 완료 시 해제)

        [Header("Sequence")]
        [SerializeField] private SequenceTextManager sequenceText; // HUD 시퀀스 출력 관리자

        [Header("Locked Messages")]
        [SerializeField, TextArea] private string[] lockedMessages; // 잠김 상태 대사(순환)
        [SerializeField] private string defaultLockedMessage = "잠겨 있는 것 같다.";
        // 대사가 비어 있을 때 사용할 기본 문구

        private int messageIndex = 0;          // 잠김 대사 순환 인덱스
        private bool isSequenceLocked = false; // 시퀀스 출력 중 상호작용 잠금

        #endregion


        #region Unity Event Method

        // 문 힌지 초기화
        private void Awake()
        {
            hinge = transform.GetChild(0);
            // 문 모델의 첫 번째 자식을 힌지로 사용한다
        }

        #endregion


        #region Custom Method

        // 퍼즐 관리자에서 호출하여 도어 잠금 해제 상태를 설정
        public void SetUnlocked(bool unlocked)
        {
            isLocked = !unlocked;
            // 퍼즐 완료 여부에 따라 잠금 상태를 반영한다
        }

        // 플레이어 상호작용 입력 처리
        public override void Interaction()
        {
            if (!interactionEnabled) return;
            // 도어 상호작용이 비활성화된 경우 처리하지 않는다

            if (isSequenceLocked) return;
            // 만약 [시퀀스 출력 중이라면] [상호작용을 차단한다]

            if (isMoving) return;
            // 문 애니메이션 중 중복 입력을 방지한다

            if (isLocked)
            {
                ShowLockedMessage();
                return;
                // 잠김 상태라면 잠김 메시지만 출력한다
            }

            StartCoroutine(MoveDoorRoutine(isOpen ? 0f : -90f));
            // 현재 문 상태에 따라 열기 또는 닫기 회전을 시작한다

            isOpen = !isOpen;
            // 문 개방 상태를 반전시킨다
        }

        // 잠김 메시지 순환 출력
        private void ShowLockedMessage()
        {
            string message = GetNextLockedMessage();
            // 출력할 잠김 메시지를 선택한다

            if (sequenceText == null) return;
            // 시퀀스 매니저가 없으면 출력하지 않는다

            StartCoroutine(ShowAndUnlock(message));
            // 출력 후 잠금을 해제하는 코루틴을 실행한다
        }

        // 잠김 메시지 선택
        private string GetNextLockedMessage()
        {
            if (lockedMessages == null || lockedMessages.Length == 0)
                return defaultLockedMessage;
            // 만약 [대사 배열이 비어 있다면] [기본 문구를 반환한다]

            string message = lockedMessages[messageIndex];
            // 현재 인덱스의 대사를 선택한다

            messageIndex++;
            // 다음 상호작용을 위해 인덱스를 증가시킨다

            if (messageIndex >= lockedMessages.Length)
                messageIndex = 0;
            // 만약 [목록의 끝에 도달했다면] [다시 처음으로 되돌린다]

            if (string.IsNullOrWhiteSpace(message))
                return defaultLockedMessage;
            // 만약 [선택된 대사가 비어 있다면] [기본 문구로 대체한다]

            return message;
        }

        // 잠김 메시지 출력 후 입력 잠금 해제
        private IEnumerator ShowAndUnlock(string message)
        {
            isSequenceLocked = true;
            // 시퀀스 출력 중 상호작용을 차단한다

            sequenceText.ShowMessage(message);
            // 잠김 메시지를 출력한다

            yield return new WaitForSeconds(0.1f);
            // 너무 빠른 연타로 중복 출력되는 것을 방지한다

            isSequenceLocked = false;
            // 상호작용 잠금을 해제한다
        }

        // 문을 부드럽게 회전시키는 코루틴
        private IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true;
            // 문이 이동 중임을 표시한다

            if (hinge == null)
            {
                isMoving = false;
                yield break;
            }
            // 만약 [힌지가 없다면] [회전을 중단한다]

            float duration = 1.0f;
            float elapsed = 0f;

            Quaternion startRot = hinge.localRotation;
            Quaternion targetRot = Quaternion.Euler(0f, targetAngle, 0f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                hinge.localRotation = Quaternion.Lerp(startRot, targetRot, elapsed / duration);
                yield return null;
                // 경과 시간에 따라 문 회전을 보간한다
            }

            hinge.localRotation = targetRot;
            // 최종 각도로 보정한다

            isMoving = false;
            // 회전 완료 후 이동 상태를 해제한다
        }

        // 외부 퍼즐에서 도어 상호작용 가능 여부를 설정한다
        public void SetInteractionEnabled(bool enabled)
        {
            interactionEnabled = enabled;
            // 상호작용 가능 상태를 설정한다
        }

        #endregion


        #region Property

        // Action UI에 표시할 상호작용 문구를 반환한다
        public string GetActionText()
        {
            return isOpen ? "문 닫기" : "문 열기";
            // 문 상태에 따라 액션 문구를 반환한다
        }

        #endregion
    }
}
