using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace FaintFear
{
    public class Door : Interactive, IActionProvider, ISaveableWorldObject
    {
        Transform hinge;
        bool isMoving = false;
        bool isOpen = false;

        [Header("Save")]
        [SerializeField] private string uniqueId; // 문 고유 ID

        [Header("Sound IDs (SoundManager 기준)")]
        [SerializeField] private string openSFX = "SFX_DoorOpen";
        [SerializeField] private string closeSFX = "SFX_DoorClose";

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
                if (!string.IsNullOrEmpty(openSFX))
                    SoundManager.Instance.PlaySFX(openSFX);
                StartCoroutine(MoveDoorRoutine(-90f));
                onDoorOpen?.Invoke();
            }
            else
            {
                if (!string.IsNullOrEmpty(closeSFX))
                    SoundManager.Instance.PlaySFX(closeSFX);
                StartCoroutine(MoveDoorRoutine(0f));
                onDoorClose?.Invoke();
            }

            isOpen = !isOpen;

            // ⭐ 런타임 상태 기록
            RuntimeStateManager.RecordDoorState(uniqueId, isOpen, false);
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
            doorState.isLocked = false; // 일반 문은 잠금 없음
        }

        public void Load(SaveData data)
        {
            var doorState = data.doorStates.Find(d => d.id == uniqueId);
            if (doorState != null)
            {
                isOpen = doorState.isOpen;

                // 문 회전 즉시 적용
                if (hinge != null)
                {
                    hinge.localRotation = Quaternion.Euler(0, isOpen ? -90f : 0f, 0);
                }
            }
        }
    }
}