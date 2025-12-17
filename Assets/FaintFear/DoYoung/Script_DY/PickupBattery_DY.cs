using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 배터리 아이템 획득하기
    /// </summary>
    public class PickupBatteryDY : Interactive
    {
        public override void Interaction()
        {
            //배터리 충전
            PlayerStatus.Instance.AddBattery(1);

            //아이템 킬
            Destroy(gameObject);
        }
    }

}
