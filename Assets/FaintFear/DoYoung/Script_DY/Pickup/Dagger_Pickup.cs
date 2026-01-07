using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 대거를 획득하는 퍼즐 전용 픽업 오브젝트
    /// 영구 퍼즐 도구로 등록된다
    /// </summary>
    public class Dagger_Pickup : PickupItemBase, IActionProvider
    {
        [Header("UI")]
        [SerializeField] private string actionText = "[E] 대거를 줍는다";

        // ===================== Pickup =====================

        protected override void OnPickup()
        {
            PuzzleInventory inventory = PuzzleInventory.Instance;
            if (inventory == null) return;

            // 대거를 영구 퍼즐 도구로 등록
            inventory.AcquireBoltCutter();
        }

        // ===================== UI =====================

        public string GetActionText()
        {
            return actionText;
        }
    }
}
