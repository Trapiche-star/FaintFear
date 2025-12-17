using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 배터리 아이템 획득 처리
    /// </summary>
    public class PickupBattery : Interactive, IActionProvider
    {
        // E 키 상호작용 시 호출
        public override void Interaction()
        {
            // 플레이어 배터리 1개 추가
            PlayerStatus.Instance.AddBattery(1);

            // 배터리 아이템 제거
            Destroy(gameObject);
        }

        // ActionUI에 표시할 문구 제공
        public string GetActionText()
        {
            // 화면에 표시될 상호작용 문구
            return "배터리";
        }
    }
}
