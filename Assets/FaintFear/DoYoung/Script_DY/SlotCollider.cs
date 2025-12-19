using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 슬롯 콜라이더 상호작용
    /// ActionUI 문구를 제공하고 E 입력 시 슬롯 삽입을 시도한다.
    /// </summary>
    public class SlotCollider : Interactive, IActionProvider
    {
        #region Variables

        // 연결된 슬롯 컨트롤러
        [SerializeField]
        private SlotController slotController;

        // UI 문구(기본)
        [SerializeField]
        private string actionText = "레버를 꽂는다";

        // 레버 없을 때 문구(선택)
        [SerializeField]
        private string needLeverText = "레버가 필요하다";

        #endregion


        #region Interactive Override

        // PlayerInteraction에서 호출됨
        public override void Interaction()
        {
            // 슬롯 컨트롤러가 없으면 중단
            if (slotController == null) return;

            // 슬롯 삽입 시도
            slotController.TryInsert();
        }

        #endregion


        #region IActionProvider

        // ActionUI 문구 제공
        public string GetActionText()
        {
            // 슬롯이 채워졌으면 문구 숨김
            if (slotController != null && slotController.IsFilled)
                return string.Empty;

            // 인벤토리 없으면 기본 문구
            if (PuzzleInventory.Instance == null || slotController == null)
                return actionText;

            // 요구 레버가 없으면 안내 문구
            if (!PuzzleInventory.Instance.HasLever(slotController.RequiredLeverIndex))
                return needLeverText;

            return actionText;
        }

        #endregion
    }
}
