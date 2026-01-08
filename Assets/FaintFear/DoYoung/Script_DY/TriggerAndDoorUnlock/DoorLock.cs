using UnityEngine;
using System.Collections;

namespace FaintFear
{
    public class DoorLock : LockedDoorBase, IActionProvider
    {
        [SerializeField] private RoomKeyType requiredKey;
        [SerializeField] private Transform hinge;

        protected override bool CanUnlock()
        {
            var player = PlayerStatus.Instance;
            if (player == null) return false;

            if (!player.HasKey(requiredKey))
            {
                //열쇠 없을 때 잠김 SFX 재생
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySFX("SFX_DoorLocked");
                return false;
            }

            // 열쇠 소비
            player.ConsumeKey(requiredKey);
            return true;
            
            //if (!player.HasKey(requiredKey)) return false;

            //player.ConsumeKey(requiredKey);
            //return true;
        }

        protected override void ToggleDoor()
        {
            // 문 열림/닫힘 SFX 재생
            if (SoundManager.Instance != null)
            {
                if (isOpen)
                    SoundManager.Instance.PlaySFX("SFX_DoorClose"); // 문 닫힘
                else
                    SoundManager.Instance.PlaySFX("SFX_DoorOpen"); // 문 열림
            }


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

        protected override void ApplyDoorRotation()
        {
            if (hinge != null)
                hinge.localRotation = Quaternion.Euler(0, isOpen ? -90f : 0f, 0);
        }

        // Action UI에 표시할 문구를 제공한다
        public string GetActionText()
        {
            return isOpen ? "[E] 문 닫기" : "[E] 문 열기";
            // 문 상태에 따라 액션 텍스트를 반환한다
        }

    }
}