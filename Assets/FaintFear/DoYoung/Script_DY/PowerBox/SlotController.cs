using UnityEngine;

namespace FaintFear
{
    public class SlotController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private int requiredLeverIndex = 0;
        [SerializeField] private GameObject insertedLever;
        [SerializeField] private PowerBoxController powerBox;

        private bool isFilled = false;

        #endregion

        #region Property

        public bool IsFilled => isFilled;
        public int RequiredLeverIndex => requiredLeverIndex;
        // ⭐ 추가: 레버 오브젝트 활성화 상태 반환
        public bool IsLeverObjectActive => insertedLever != null && insertedLever.activeSelf;

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

            if (EndingManager.Instance != null)
                EndingManager.Instance.SetLeverActivated(requiredLeverIndex);

            if (ElevatorManager.Instance != null && requiredLeverIndex == 0)
                ElevatorManager.Instance.SupplyPower();

            if (powerBox != null)
                powerBox.CheckPuzzleComplete();

            // ⭐ 추가: 레버 삽입 시 런타임 상태 기록
            if (powerBox != null)
                powerBox.RecordPowerBoxStateToRuntime();
        }

        // ⭐ 수정: 레버 오브젝트 활성화 상태도 함께 복구
        public void RestoreFilledState()
        {
            RestoreFilledState(true, true, true);
        }

        public void RestoreFilledState(bool filled, bool notifyManagers = true)
        {
            RestoreFilledState(filled, filled, notifyManagers);
        }

        // ⭐ 추가: 레버 오브젝트 활성화 상태를 별도로 제어할 수 있는 오버로드
        public void RestoreFilledState(bool filled, bool leverObjectActive, bool notifyManagers = true)
        {
            isFilled = filled;

            // ⭐ 레버 오브젝트는 leverObjectActive 파라미터에 따라 설정
            if (insertedLever != null)
                insertedLever.SetActive(leverObjectActive);

            if (!filled)
                return;

            if (!notifyManagers)
                return;

            if (EndingManager.Instance != null)
                EndingManager.Instance.SetLeverActivated(requiredLeverIndex);

            if (ElevatorManager.Instance != null && requiredLeverIndex == 0)
                ElevatorManager.Instance.SupplyPower();

            if (powerBox != null)
                powerBox.CheckPuzzleComplete();
        }

        #endregion
    }
}