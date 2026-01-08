using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 지하실 도어 상호작용 오브젝트
    /// 전역 해제 여부에 따라 잠김 메시지 또는 문 개방을 처리한다
    /// </summary>
    public class BasementDoorUnlock : LockedDoorBase, IActionProvider
    {
        #region Variables

        [Header("Door Type")]
        [SerializeField] private bool isDoubleDoor = false; // 양문 여부

        [Header("Single Door")]
        [SerializeField] private Transform hinge; // 단문 회전 담당

        [Header("Double Door")]
        [SerializeField] private Transform leftHinge;  // 양문 왼쪽 회전 담당
        [SerializeField] private Transform rightHinge; // 양문 오른쪽 회전 담당

        [Header("Open Angles")]
        [SerializeField] private float singleOpenAngle = -90f; // 단문 열림 각도
        [SerializeField] private float leftOpenAngle = -90f;   // 양문 왼쪽 열림 각도
        [SerializeField] private float rightOpenAngle = 90f;   // 양문 오른쪽 열림 각도

        [Header("Locked Messages")]
        [SerializeField] private string[] lockedMessages;              // 잠김 상태에서 순차 출력될 문장들
        [SerializeField] private string defaultLockedMessage = "잠겨 있는 것 같다."; // 기본 메시지

        private int messageIndex = 0; // 잠김 메시지 출력 인덱스

        #endregion


        #region Custom Method

        // 전역 상태를 기준으로 잠금 해제 가능 여부를 판단한다
        protected override bool CanUnlock()
        {
            if (BasementDoorManager.Instance == null) return false;
            // 만약 매니저 인스턴스가 없다면 이 문에서는 더 이상 상호작용하지 않는다

            //if (!BasementDoorManager.Instance.IsBasementDoorUnlocked) return false;
            // 만약 지하실 도어가 아직 해제되지 않았다면 이 문에서는 더 이상 상호작용하지 않는다

            if (!BasementDoorManager.Instance.IsBasementDoorUnlocked)
            {
                // + 잠김 상태일 때 SFX 재생
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySFX("SFX_DoorLocked");

                ShowLockedMessage();
                return false;
            }

            return true;
            // 전역 해제 상태이므로 문을 열 수 있다
        }

        // 문을 열거나 닫는 동작을 수행한다
        protected override void ToggleDoor()
        {
            //+ 문 열림/닫힘 SFX
            if (SoundManager.Instance != null)
            {
                if (!isOpen)
                    SoundManager.Instance.PlaySFX("SFX_DoorOpen"); // 문 열림
                else
                    SoundManager.Instance.PlaySFX("SFX_DoorClose"); // 문 닫힘
            }

            StartCoroutine(MoveDoorRoutine(isOpen));
            // 현재 상태를 기준으로 열기 또는 닫기 애니메이션을 실행한다

            isOpen = !isOpen;
            // 문 상태를 반전시킨다
        }

        // 문 회전 애니메이션을 처리한다
        private IEnumerator MoveDoorRoutine(bool opened)
        {
            isMoving = true;
            // 문 이동이 시작되었으므로 이동 중 상태로 설정한다

            float duration = 1f;
            float elapsed = 0f;

            Quaternion startSingle = Quaternion.identity;
            Quaternion startLeft = Quaternion.identity;
            Quaternion startRight = Quaternion.identity;

            Quaternion targetSingle = Quaternion.identity;
            Quaternion targetLeft = Quaternion.identity;
            Quaternion targetRight = Quaternion.identity;

            if (!isDoubleDoor)
            {
                if (hinge == null) yield break;
                // 만약 단문 힌지가 없다면 이 코루틴에서는 더 이상 처리하지 않는다

                startSingle = hinge.localRotation;
                targetSingle = Quaternion.Euler(0, opened ? 0 : singleOpenAngle, 0);
            }
            else
            {
                if (leftHinge == null || rightHinge == null) yield break;
                // 만약 양문 힌지가 하나라도 없다면 이 코루틴에서는 더 이상 처리하지 않는다

                startLeft = leftHinge.localRotation;
                startRight = rightHinge.localRotation;

                targetLeft = Quaternion.Euler(0, opened ? 0 : leftOpenAngle, 0);
                targetRight = Quaternion.Euler(0, opened ? 0 : rightOpenAngle, 0);
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                // 경과 시간을 프레임 단위로 누적한다

                float t = elapsed / duration;

                if (!isDoubleDoor)
                {
                    hinge.localRotation = Quaternion.Lerp(startSingle, targetSingle, t);
                    // 단문 회전을 목표 각도로 부드럽게 보간한다
                }
                else
                {
                    leftHinge.localRotation = Quaternion.Lerp(startLeft, targetLeft, t);
                    rightHinge.localRotation = Quaternion.Lerp(startRight, targetRight, t);
                    // 양문 좌우를 동시에 보간 회전시킨다
                }

                yield return null;
                // 다음 프레임까지 대기한다
            }

            if (!isDoubleDoor)
            {
                hinge.localRotation = targetSingle;
                // 단문 회전을 최종 값으로 보정한다
            }
            else
            {
                leftHinge.localRotation = targetLeft;
                rightHinge.localRotation = targetRight;
                // 양문 좌우 회전을 최종 값으로 보정한다
            }

            isMoving = false;

            //+ 지하실 언락 후 문 열림일 때 SFX_Jumpscare01
            if (!opened)
            {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySFX("SFX_Jumpscare01");
            }

            // 문 이동이 완료되었음을 표시한다
        }

        // 잠겨 있을 때 출력할 메시지를 처리한다
        protected override void ShowLockedMessage()
        {
            if (sequenceText == null) return;
            // 만약 시퀀스 텍스트 매니저가 없다면 이 메서드에서는 더 이상 처리하지 않는다

            string msg = lockedMessages != null && lockedMessages.Length > 0
                ? lockedMessages[messageIndex]
                : defaultLockedMessage;

            messageIndex++;
            // 다음 메시지를 가리키도록 인덱스를 증가시킨다

            if (lockedMessages != null && messageIndex >= lockedMessages.Length)
                messageIndex = 0;
            // 만약 마지막 메시지까지 출력했다면 다시 처음으로 되돌린다

            sequenceText.ShowMessage(string.IsNullOrWhiteSpace(msg) ? defaultLockedMessage : msg);
            // 메시지가 비어 있다면 기본 문구를 대신 출력한다
        }

        // 현재 문 상태에 따라 회전을 즉시 반영한다
        protected override void ApplyDoorRotation()
        {
            if (!isDoubleDoor)
            {
                if (hinge != null)
                    hinge.localRotation = Quaternion.Euler(0, isOpen ? singleOpenAngle : 0, 0);
                // 단문일 경우 상태에 맞는 각도를 즉시 적용한다
            }
            else
            {
                if (leftHinge != null)
                    leftHinge.localRotation = Quaternion.Euler(0, isOpen ? leftOpenAngle : 0, 0);
                // 양문 왼쪽 회전을 적용한다

                if (rightHinge != null)
                    rightHinge.localRotation = Quaternion.Euler(0, isOpen ? rightOpenAngle : 0, 0);
                // 양문 오른쪽 회전을 적용한다
            }
        }

        // Action UI에 표시할 문구를 제공한다
        public string GetActionText()
        {
            return isOpen ? "[E] 문 닫기" : "[E] 문 열기";
            // 문 상태에 따라 액션 텍스트를 반환한다
        }

        #endregion
    }
}
