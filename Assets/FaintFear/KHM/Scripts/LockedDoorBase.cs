using UnityEngine;
using System.Collections;

namespace FaintFear
{
    public abstract class LockedDoorBase : Interactive, ISaveableWorldObject
    {
        [Header("Door State")]
        [SerializeField] protected bool isLocked = true;
        [SerializeField] protected bool isOpen = false;
        [SerializeField] protected string uniqueId;

        [Header("Sequence")]
        [SerializeField] protected SequenceTextManager sequenceText;

        protected bool isMoving = false;

        // ⭐ 이미 체크포인트로 저장됐는지 추적
        private bool wasSavedAsCheckpoint = false;

        // 잠긴 문 상호작용
        public override void Interaction()
        {
            if (isMoving) return;

            if (isLocked)
            {
                if (!CanUnlock())
                {
                    ShowLockedMessage();
                    return;
                }
                UnlockDoor();

                // ⭐ 잠금 해제 시에만 저장 (최초 1회)
                RecordUnlockState();
                return; // 잠금 해제만 하고 문은 안 열림
            }

            ToggleDoor();

            // ⭐ 문 열림/닫힘은 런타임 상태만 기록 (파일 저장 X)
            RuntimeStateManager.RecordDoorState(uniqueId, isOpen, isLocked);
        }

        // 잠금 해제 조건 체크 (각 자식이 구현)
        protected abstract bool CanUnlock();

        // 잠금 해제 처리 (공통)
        protected virtual void UnlockDoor()
        {
            isLocked = false;
            ShowUnlockedMessage();
        }

        // 문 열기/닫기 처리 (공통)
        protected abstract void ToggleDoor();

        protected virtual void ShowLockedMessage()
        {
            if (sequenceText != null)
                sequenceText.ShowMessage("잠겨 있는 문이다.");
        }

        protected virtual void ShowUnlockedMessage()
        {
            if (sequenceText != null)
                sequenceText.ShowMessage("문이 잠금 해제되었다.");
        }

        // ⭐ 잠금 해제 시에만 호출 (최초 1회만 저장) - protected로 변경
        protected void RecordUnlockState()
        {
            RuntimeStateManager.RecordDoorState(uniqueId, isOpen, isLocked);

            if (!wasSavedAsCheckpoint)
            {
                wasSavedAsCheckpoint = true;
                AutoSaveManager.Instance?.RequestSave($"door_unlock_{uniqueId}");
                Debug.Log($"[LockedDoorBase] 잠금 해제 - 자동저장 요청: {uniqueId}");
            }
        }

        // ==================== ISaveableWorldObject ====================
        public string GetID() => uniqueId;

        public virtual void Save(ref SaveData data)
        {
            var doorState = data.doorStates.Find(d => d.id == uniqueId);
            if (doorState == null)
            {
                doorState = new DoorStateData { id = uniqueId };
                data.doorStates.Add(doorState);
            }

            doorState.isOpen = isOpen;
            doorState.isLocked = isLocked;
            doorState.wasSaved = wasSavedAsCheckpoint;
        }

        public virtual void Load(SaveData data)
        {
            var doorState = data.doorStates.Find(d => d.id == uniqueId);
            if (doorState != null)
            {
                isOpen = doorState.isOpen;
                isLocked = doorState.isLocked;
                wasSavedAsCheckpoint = doorState.wasSaved;
                ApplyDoorRotation();
            }
        }

        public void ForceUnlockFromKeypad()
        {
            if (!isLocked) return;

            isLocked = false;
            ShowUnlockedMessage();

            // 잠금 해제 상태 저장
            RecordUnlockState();
        }

        protected abstract void ApplyDoorRotation();
    }
}