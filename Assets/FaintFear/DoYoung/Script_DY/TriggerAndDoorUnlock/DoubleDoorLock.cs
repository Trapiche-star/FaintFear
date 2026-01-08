using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 양문 도어 컨트롤러
    /// 문이 실제로 열릴 때 지하실 도어 해제 트리거를 호출한다
    /// </summary>
    public class DoubleDoorLock : LockedDoorBase, IActionProvider
    {
        #region Variables

        [Header("Door Hinges")]
        [SerializeField] private Transform leftHinge;   // 왼쪽 문 회전을 담당
        [SerializeField] private Transform rightHinge;  // 오른쪽 문 회전을 담당

        [Header("Open Angles")]
        [SerializeField] private float leftOpenAngle = -90f;   // 왼쪽 문 열림 각도
        [SerializeField] private float rightOpenAngle = 90f;   // 오른쪽 문 열림 각도

        [Header("Key Settings")]
        [SerializeField] private RoomKeyType requiredKey;       // 문 개방에 필요한 열쇠 타입

        [Header("Basement Unlock")]
        [SerializeField] private BasementDoorUnlockTrigger unlockTrigger; // 지하실 도어 해제 트리거

        private bool hasSentUnlockSignal = false; // 언락 신호 중복 전송 방지

        #endregion


        #region Custom Method

        // 열쇠 보유 여부를 검사하여 잠금 해제 가능 여부를 판단한다
        protected override bool CanUnlock()
        {
            var player = PlayerStatus.Instance;
            if (player == null) return false;
            // 만약 플레이어 인스턴스가 없다면 이 문에서는 더 이상 상호작용하지 않는다

            // + 열쇠 없을 때 잠김 SFX 재생
            if (!player.HasKey(requiredKey))
            {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySFX("SFX_DoorLocked"); // 문 잠김
                return false;
            }
            //if (!player.HasKey(requiredKey)) return false;
            // 만약 필요한 열쇠를 보유하지 않았다면 이 문에서는 더 이상 상호작용하지 않는다

            player.ConsumeKey(requiredKey);
            // 조건을 만족했으므로 열쇠를 소모한다

            return true;
            // 모든 조건이 충족되었으므로 문을 열 수 있다
        }

        // 문을 열거나 닫는 동작을 수행한다
        protected override void ToggleDoor()
        {
            // + 문 열림/닫힘 SFX 재생
            if (SoundManager.Instance != null)
            {
                if (isOpen)
                    SoundManager.Instance.PlaySFX("SFX_DoorClose"); // 문 닫힘
                else
                    SoundManager.Instance.PlaySFX("SFX_DoorOpen"); // 문 열림
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
            // 만약 문 이동을 시작했다면 이 문은 이동 중 상태로 전환한다

            float duration = 1f;
            // 회전 애니메이션 지속 시간을 설정한다

            float elapsed = 0f;
            // 경과 시간을 초기화한다

            Quaternion leftStart = leftHinge.localRotation;
            Quaternion rightStart = rightHinge.localRotation;
            // 현재 좌우 문 회전 상태를 시작값으로 저장한다

            Quaternion leftTarget = Quaternion.Euler(0, opened ? 0 : leftOpenAngle, 0);
            Quaternion rightTarget = Quaternion.Euler(0, opened ? 0 : rightOpenAngle, 0);
            // 현재 상태에 따라 열림 또는 닫힘 목표 회전값을 계산한다

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                // 경과 시간을 프레임 단위로 누적한다

                leftHinge.localRotation = Quaternion.Lerp(leftStart, leftTarget, elapsed / duration);
                rightHinge.localRotation = Quaternion.Lerp(rightStart, rightTarget, elapsed / duration);
                // 좌우 문을 목표 각도로 부드럽게 보간 회전시킨다

                yield return null;
                // 다음 프레임까지 대기한다
            }

            leftHinge.localRotation = leftTarget;
            rightHinge.localRotation = rightTarget;
            // 애니메이션 종료 시 최종 회전값을 정확히 적용한다

            isMoving = false;

            // + 지하실 언락 처리
            if (!opened && !hasSentUnlockSignal)
            {
                hasSentUnlockSignal = true;
                if (unlockTrigger != null)
                    unlockTrigger.TriggerUnlock();
            }

            /*
            // 문 이동이 완료되었으므로 이동 상태를 해제한다

            if (!opened && !hasSentUnlockSignal)
            {
                // 만약 이번 동작이 '닫힘 → 열림'이고 아직 신호를 보낸 적이 없다면 지하실 언락을 수행한다

                hasSentUnlockSignal = true;
                // 이후 중복 호출을 방지하기 위해 신호 전송 상태를 기록한다

                if (unlockTrigger != null)
                    unlockTrigger.TriggerUnlock();
                // 지하실 도어 해제를 전역 상태로 기록한다
            }
            */
        }

        // 현재 문 상태에 따라 회전을 즉시 반영한다
        protected override void ApplyDoorRotation()
        {
            if (leftHinge != null)
                leftHinge.localRotation = Quaternion.Euler(0, isOpen ? leftOpenAngle : 0, 0);
            // 왼쪽 문을 현재 상태에 맞는 각도로 즉시 적용한다

            if (rightHinge != null)
                rightHinge.localRotation = Quaternion.Euler(0, isOpen ? rightOpenAngle : 0, 0);
            // 오른쪽 문을 현재 상태에 맞는 각도로 즉시 적용한다
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
