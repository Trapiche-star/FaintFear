using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 배터리 아이템 획득하기
    /// </summary>
    public class PickupBattery : Interactive
    {
        #region Variablse
        [SerializeField]
        private float chargeBattery = 20f;   //배터리 충전량
        #endregion

        #region Custom Method
        public override void Interaction()
        {
            //배터리 충전
            PlayerStatus.Instance.AddBattery(chargeBattery);

            //아이템 킬
            Destroy(gameObject);
        }
        #endregion
    }
}


