using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace FaintFear
{
    public class Door : Interactive, IActionProvider
    {
        Transform hinge;
        bool isMoving = false;
        bool isOpen = false;

        [Header("Events")]
        public UnityEvent onDoorOpen;
        public UnityEvent onDoorClose;

        private void Awake()
        {
            hinge = transform.GetChild(0);
        }

        public override void Interaction()
        {
            if (isMoving) return;

            if (!isOpen)
            {
                //문 열림 사운드
                SoundManager.Instance.PlaySFX("SFX_DoorOpen");

                StartCoroutine(MoveDoorRoutine(-90f));
                onDoorOpen?.Invoke();
            }
            else
            {
                //문 닫힘 사운드
                SoundManager.Instance.PlaySFX("SFX_DoorClose");

                StartCoroutine(MoveDoorRoutine(0f));
                onDoorClose?.Invoke();
            }

            isOpen = !isOpen;
        }

        IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true;

            float duration = 1f;
            float t = 0f;

            Quaternion start = hinge.localRotation;
            Quaternion target = Quaternion.Euler(0, targetAngle, 0);

            while (t < duration)
            {
                t += Time.deltaTime;
                hinge.localRotation = Quaternion.Lerp(start, target, t / duration);
                yield return null;
            }

            hinge.localRotation = target;
            isMoving = false;
        }

        public string GetActionText()
        {
            return isOpen ? "닫기" : "열기";
        }
    }
}
