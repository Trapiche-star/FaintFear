using UnityEngine;

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
        Flashlight flashlight = FindFirstObjectByType<Flashlight>();

        if (flashlight != null)
        {
            flashlight.AddBattery(chargeBattery);
        }

        //아이템 킬
        Destroy(gameObject);    
    }
    #endregion
}
