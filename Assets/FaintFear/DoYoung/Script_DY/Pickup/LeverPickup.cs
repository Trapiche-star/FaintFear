using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 레버 아이템 픽업 처리
    /// 퍼즐 인벤토리에 레버를 추가한다.
    /// </summary>
    public class LeverPickup : PickupItemBase, IActionProvider
    {
        #region Variables

        [Header("Lever Settings")]
        [SerializeField] private int leverIndex;

        [SerializeField] private string actionText = "[E] 레버";

        #endregion

        // ===================== Pickup =====================

        protected override void OnPickup()
        {
            if (PuzzleInventory.Instance == null)
            {
                Debug.LogWarning("[LeverPickup] PuzzleInventory가 없습니다.");
                return;
            }

            PuzzleInventory.Instance.AddLever(leverIndex);
        }

        // ===================== IActionProvider =====================

        public string GetActionText()
        {
            return actionText;
        }
    }
}
