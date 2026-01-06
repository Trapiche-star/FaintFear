using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 배터리 아이템 획득 처리
    /// </summary>
    public class PickupBattery : PickupItemBase, IActionProvider
    {
        protected override void OnPickup()
        {
            PlayerStatus.Instance.AddBattery(1);
        }

        public string GetActionText()
        {
            return "[E] 배터리";
        }
    }
}
