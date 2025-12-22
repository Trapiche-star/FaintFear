using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 슬롯 상태 컨트롤러
    /// 레버 요구 조건을 검사하고 성공 시 슬롯을 채운다.
    /// </summary>
    public class SlotController : MonoBehaviour
    {
        #region Variables

        // 이 슬롯이 요구하는 레버 번호 (0~3)
        [SerializeField]
        private int requiredLeverIndex = 0;

        // 슬롯에 레버가 꽂혔을 때 보여줄 오브젝트 (기본 OFF)
        [SerializeField]
        private GameObject insertedLever;

        // 파워박스 퍼즐 관리자 (완료 여부 체크용)
        [SerializeField]
        private PowerBoxController powerBox;

        // 슬롯이 이미 채워졌는지 상태
        private bool isFilled = false;

        #endregion


        #region Properties

        // 슬롯이 채워졌는지 외부에서 확인할 수 있게 한다
        public bool IsFilled => isFilled;

        // 이 슬롯이 요구하는 레버 번호를 외부에 제공한다
        public int RequiredLeverIndex => requiredLeverIndex;

        #endregion


        #region Public Methods

        // 슬롯에 레버를 삽입하려고 시도한다
        public bool TryInsert()
        {
            // 이미 슬롯이 채워져 있으면 더 이상 처리하지 않는다
            if (isFilled) return false;

            // 퍼즐 인벤토리가 없으면 시도 자체를 중단한다
            if (PuzzleInventory.Instance == null) return false;

            // 이 슬롯이 요구하는 레버를 가지고 있는지 확인한다
            if (!PuzzleInventory.Instance.HasLever(requiredLeverIndex))
                return false; // 다른 레버이거나 아예 레버가 없으므로 실패

            // 요구 레버가 맞다면 해당 레버를 소비한다
            PuzzleInventory.Instance.ConsumeLever(requiredLeverIndex);

            // 슬롯을 채우는 실제 처리를 실행한다
            FillSlot();

            // 정상적으로 삽입되었음을 알린다
            return true;
        }

        // 슬롯을 채웠을 때의 실제 처리
        private void FillSlot()
        {
            // 중복 호출을 방지한다
            if (isFilled) return;

            // 슬롯 상태를 채워진 상태로 변경한다
            isFilled = true;

            // 슬롯에 맞는 레버 오브젝트를 화면에 표시한다
            if (insertedLever != null)
                insertedLever.SetActive(true);

            // 퍼즐 전체 완료 여부를 다시 검사하도록 보고한다
            if (powerBox != null)
                powerBox.CheckPuzzleComplete();
        }

        #endregion
    }
}
