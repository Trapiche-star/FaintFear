using UnityEngine;
using System.Collections;

namespace FaintFear
{
    public class DoubleDoorLock : LockedDoorBase, IActionProvider
    {
        [Header("Door Hinges")]
        [SerializeField] private Transform leftHinge;
        [SerializeField] private Transform rightHinge;

        [Header("Open Angles")]
        [SerializeField] private float leftOpenAngle = -90f;
        [SerializeField] private float rightOpenAngle = 90f;

        [Header("Key Settings")]
        [SerializeField] private RoomKeyType requiredKey;

        protected override bool CanUnlock()
        {
            var player = PlayerStatus.Instance;
            if (player == null) return false;

            if (!player.HasKey(requiredKey)) return false;

            player.ConsumeKey(requiredKey);
            return true;
        }

        protected override void ToggleDoor()
        {
            StartCoroutine(MoveDoorRoutine(isOpen));
            isOpen = !isOpen;
        }

        private IEnumerator MoveDoorRoutine(bool opened)
        {
            isMoving = true;
            float duration = 1f;
            float elapsed = 0f;

            Quaternion leftStart = leftHinge.localRotation;
            Quaternion rightStart = rightHinge.localRotation;

            Quaternion leftTarget = Quaternion.Euler(0, opened ? 0 : leftOpenAngle, 0);
            Quaternion rightTarget = Quaternion.Euler(0, opened ? 0 : rightOpenAngle, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                leftHinge.localRotation = Quaternion.Lerp(leftStart, leftTarget, elapsed / duration);
                rightHinge.localRotation = Quaternion.Lerp(rightStart, rightTarget, elapsed / duration);
                yield return null;
            }

            leftHinge.localRotation = leftTarget;
            rightHinge.localRotation = rightTarget;
            isMoving = false;
        }

        protected override void ApplyDoorRotation()
        {
            if (leftHinge != null) leftHinge.localRotation = Quaternion.Euler(0, isOpen ? leftOpenAngle : 0, 0);
            if (rightHinge != null) rightHinge.localRotation = Quaternion.Euler(0, isOpen ? rightOpenAngle : 0, 0);
        }

        // Action UI에 표시할 문구를 제공한다
        public string GetActionText()
        {
            return isOpen ? "[E] 문 닫기" : "[E] 문 열기";
            // 문 상태에 따라 액션 텍스트를 반환한다
        }
    }
}
