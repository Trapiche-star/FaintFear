using UnityEngine;

namespace FaintFear
{
    public class PowerBoxController : MonoBehaviour, ISaveableWorldObject
    {
        #region Variables

        [Header("Save")]
        [SerializeField] private string uniqueId = "PowerBox_Main";

        [SerializeField] private SlotController[] slots;
        [SerializeField] private SlotCollider[] slotColliders;
        [SerializeField] private SequenceTextManager sequenceText;

        private bool isCompleted = false;
        private bool isPowerSupplied = false;

        // ⭐ 이미 체크포인트로 저장됐는지 추적
        private bool wasSavedAsCheckpoint = false;

        #endregion

        #region Unity Event Method

        private void Start()
        {
            SetSlotTriggerActive(false);

            if (slotColliders.Length > 0)
                slotColliders[0].enabled = true;
        }

        #endregion

        #region Public Methods

        public void CheckPuzzleComplete()
        {
            if (slots.Length == 0) return;

            if (!isPowerSupplied && slots[0].IsFilled)
            {
                SupplyPower();
            }

            if (isCompleted) return;

            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsFilled)
                    return;
            }

            OnPuzzleCompleted();
        }

        public void SetSlotTriggerActive(bool isActive)
        {
            for (int i = 0; i < slotColliders.Length; i++)
            {
                if (slotColliders[i] != null)
                    slotColliders[i].enabled = isActive;
            }
        }

        #endregion

        #region Private Methods

        private void SupplyPower()
        {
            isPowerSupplied = true;

            for (int i = 1; i < slotColliders.Length; i++)
            {
                if (slotColliders[i] != null)
                    slotColliders[i].enabled = true;
            }

            if (sequenceText != null)
                sequenceText.ShowMessage("일부 시설들에 전력이 들어온 것 같다.");

            // ⭐ 런타임 상태 기록 (메인 전력 공급)
            RecordPowerBoxState();
        }

        private void OnPuzzleCompleted()
        {
            isCompleted = true;
            SetSlotTriggerActive(false);

            Debug.Log("[PowerBox] 퍼즐 완료");

            // ⭐ 퍼즐 완료 시 런타임 상태 기록 및 자동 저장
            RecordPowerBoxState();

            if (!wasSavedAsCheckpoint)
            {
                wasSavedAsCheckpoint = true;
                AutoSaveManager.Instance?.RequestSave($"powerbox_complete_{uniqueId}");
                Debug.Log($"[PowerBox] 퍼즐 완료 - 자동저장 요청");
            }
        }

        // ⭐ 런타임 상태 기록
        private void RecordPowerBoxState()
        {
            RuntimeStateManager.RecordPowerBoxState(uniqueId, GetFilledSlots(), isPowerSupplied, isCompleted);
        }

        private bool[] GetFilledSlots()
        {
            bool[] filled = new bool[slots.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                filled[i] = slots[i].IsFilled;
            }
            return filled;
        }

        #endregion

        #region Property

        public bool IsPowerSupplied => isPowerSupplied;

        #endregion

        // ⭐ ISaveableWorldObject 구현
        public string GetID() => uniqueId;

        public void Save(ref SaveData data)
        {
            data.powerBoxData.filledSlots = GetFilledSlots();
            data.powerBoxData.isPowerSupplied = isPowerSupplied;
            data.powerBoxData.isCompleted = isCompleted;
        }

        public void Load(SaveData data)
        {
            isPowerSupplied = data.powerBoxData.isPowerSupplied;
            isCompleted = data.powerBoxData.isCompleted;

            // 슬롯 상태 복원
            for (int i = 0; i < slots.Length && i < data.powerBoxData.filledSlots.Length; i++)
            {
                if (data.powerBoxData.filledSlots[i])
                {
                    slots[i].RestoreFilledState();
                }
            }

            // 전력 공급 상태에 따라 슬롯 활성화
            if (isPowerSupplied)
            {
                for (int i = 1; i < slotColliders.Length; i++)
                {
                    if (slotColliders[i] != null)
                        slotColliders[i].enabled = true;
                }
            }

            // 퍼즐 완료 시 모든 슬롯 비활성화
            if (isCompleted)
            {
                SetSlotTriggerActive(false);
            }
        }
    }
}