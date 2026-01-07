using UnityEngine;

namespace FaintFear
{
    public class SlotController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private int requiredLeverIndex = 0;
        [SerializeField] private GameObject insertedLever;
        [SerializeField] private PowerBoxController powerBox;
        [SerializeField] private EndingManager endingManager;
        [SerializeField] private ElevatorManager elevatorManager;

        private bool isFilled = false;

        #endregion

        #region Property

        public bool IsFilled => isFilled;
        public int RequiredLeverIndex => requiredLeverIndex;

        #endregion

        #region Custom Method

        public bool TryInsert()
        {
            if (isFilled)
                return false;

            if (PuzzleInventory.Instance == null)
                return false;

            if (!PuzzleInventory.Instance.HasLever(requiredLeverIndex))
                return false;

            PuzzleInventory.Instance.ConsumeLever(requiredLeverIndex);
            FillSlot();
            return true;
        }

        private void FillSlot()
        {
            if (isFilled)
                return;

            isFilled = true;

            if (insertedLever != null)
                insertedLever.SetActive(true);

            if (endingManager != null)
                endingManager.SetLeverActivated(requiredLeverIndex);

            if (elevatorManager != null && requiredLeverIndex == 0)
                elevatorManager.SupplyPower();

            if (powerBox != null)
                powerBox.CheckPuzzleComplete();
        }

        // ⭐ 저장된 상태 복원용 (레버 소모 없이 슬롯만 채움)
        public void RestoreFilledState()
        {
            isFilled = true;

            if (insertedLever != null)
                insertedLever.SetActive(true);

            Debug.Log($"[SlotController] 슬롯 상태 복원: Index {requiredLeverIndex}");
        }

        #endregion
    }
}