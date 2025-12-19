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

        // 슬롯이 요구하는 레버 번호 (0~3)
        [SerializeField]
        private int requiredLeverIndex = 0;

        // 슬롯에 꽂혔을 때 보여줄 레버 오브젝트(기본 OFF)
        [SerializeField]
        private GameObject insertedLever;

        // 퍼즐 관리자(완료 체크용)
        [SerializeField]
        private PowerBoxController powerBox;

        // 슬롯이 채워졌는지 상태
        private bool isFilled = false;

        #endregion


        #region Properties

        // 슬롯 완료 여부
        public bool IsFilled => isFilled;

        // 슬롯 요구 레버 번호
        public int RequiredLeverIndex => requiredLeverIndex;

        #endregion


        #region Public Methods

        // 슬롯 삽입 시도 (SlotCollider에서 호출)
        public bool TryInsert()
        {
            // 이미 채워졌으면 실패
            if (isFilled) return false;

            // 인벤토리가 없으면 실패
            if (PuzzleInventory.Instance == null) return false;

            // 레버 소비 시도 (없으면 실패)
            if (!PuzzleInventory.Instance.ConsumeLever(requiredLeverIndex))
                return false;

            // 슬롯 채우기
            FillSlot();
            return true;
        }

        // 슬롯을 채우는 실제 처리
        public void FillSlot()
        {
            // 중복 방지
            if (isFilled) return;

            // 상태 변경
            isFilled = true;

            // 레버 오브젝트 표시
            if (insertedLever != null)
                insertedLever.SetActive(true);

            // 퍼즐 완료 체크 보고
            if (powerBox != null)
                powerBox.CheckPuzzleComplete();
        }

        #endregion
    }
}
