using UnityEngine;
using System.Collections;

namespace FaintFear
{
    public class DoubleDoor : Interactive, IActionProvider, ISaveableWorldObject
    {
        #region Variables

        Transform leftDoor;
        Transform rightDoor;
        bool isMoving = false;
        bool isOpen = false;

        [Header("Save")]
        [SerializeField] private string uniqueId;

        [Header("Door Settings")]
        [SerializeField] float openAngle = 90f;
        [SerializeField] float duration = 1.0f;

        #endregion

        private void Awake()
        {
            leftDoor = transform.GetChild(0);
            rightDoor = transform.GetChild(1);
        }

        public override void Interaction()
        {
            if (isMoving) return;

            // + 문 열림/닫힘 SFX 재생
            if (SoundManager.Instance != null)
            {
                if (!isOpen)
                    SoundManager.Instance.PlaySFX("SFX_DoorOpen"); // 문 열림
                else
                    SoundManager.Instance.PlaySFX("SFX_DoorClose"); // 문 닫힘
            }

            if (!isOpen)
            {
                StartCoroutine(MoveDoorsRoutine(-openAngle, openAngle));
            }
            else
            {
                StartCoroutine(MoveDoorsRoutine(0f, 0f));
            }
            
            isOpen = !isOpen;

            // ⭐ 런타임 상태 기록
            RuntimeStateManager.RecordDoorState(uniqueId, isOpen, false);
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

        public string GetActionText()
        {
            return isOpen ? "[E] 문 닫기" : "[E] 문 열기";
        }

        // ⭐ ISaveableWorldObject 구현
        public string GetID() => uniqueId;

        public void Save(ref SaveData data)
        {
            var doorState = data.doorStates.Find(d => d.id == uniqueId);
            if (doorState == null)
            {
                doorState = new DoorStateData { id = uniqueId };
                data.doorStates.Add(doorState);
            }

            doorState.isOpen = isOpen;
            doorState.isLocked = false;
        }

        public void Load(SaveData data)
        {
            var doorState = data.doorStates.Find(d => d.id == uniqueId);
            if (doorState != null)
            {
                isOpen = doorState.isOpen;

                // 문 회전 즉시 적용
                if (leftDoor != null && rightDoor != null)
                {
                    if (isOpen)
                    {
                        leftDoor.localRotation = Quaternion.Euler(0, -openAngle, 0);
                        rightDoor.localRotation = Quaternion.Euler(0, openAngle, 0);
                    }
                    else
                    {
                        leftDoor.localRotation = Quaternion.Euler(0, 0, 0);
                        rightDoor.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                }
            }
        }
    }
}