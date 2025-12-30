using UnityEngine;
using System.Collections;

namespace FaintFear
{
    public class DoubleDoor : Interactive, IActionProvider
    {
        #region Variables
        Transform leftDoor;
        Transform rightDoor;

        bool isMoving = false;
        bool isOpen = false;

        [Header("Door Settings")]
        [SerializeField] float openAngle = 90f;
        [SerializeField] float duration = 1.0f;
        #endregion

        private void Awake()
        {
            // 자식 0 = 왼쪽, 자식 1 = 오른쪽
            leftDoor = transform.GetChild(0);
            rightDoor = transform.GetChild(1);
        }

        public override void Interaction()
        {
            if (isMoving) return;

            if (!isOpen)
            {
                // 열기
                StartCoroutine(MoveDoorsRoutine(-openAngle, openAngle));
            }
            else
            {
                // 닫기
                StartCoroutine(MoveDoorsRoutine(0f, 0f));
            }

            isOpen = !isOpen;
        }

        IEnumerator MoveDoorsRoutine(float leftTargetY, float rightTargetY)
        {
            isMoving = true;

            float elapsed = 0f;

            Quaternion leftStart = leftDoor.localRotation;
            Quaternion rightStart = rightDoor.localRotation;

            Quaternion leftTarget = Quaternion.Euler(0, leftTargetY, 0);
            Quaternion rightTarget = Quaternion.Euler(0, rightTargetY, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                leftDoor.localRotation = Quaternion.Lerp(leftStart, leftTarget, t);
                rightDoor.localRotation = Quaternion.Lerp(rightStart, rightTarget, t);

                yield return null;
            }

            leftDoor.localRotation = leftTarget;
            rightDoor.localRotation = rightTarget;

            isMoving = false;
        }

        // Action UI에 표시될 문구 제공
        public string GetActionText()
        {
            return isOpen ? "닫기" : "열기";
        }
    }
}

