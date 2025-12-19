using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 레버 아이템 픽업 처리
    /// 퍼즐 인벤토리에 레버를 추가한다.
    /// </summary>
    public class LeverPickup : Interactive
    {
        #region Variables

        // 이 레버의 번호 (0~3)
        [SerializeField]
        private int leverIndex;

        #endregion


        #region Interactive Override

        // 플레이어 상호작용 시 호출
        public override void Interaction()
        {
            // 퍼즐 인벤토리가 없으면 중단
            if (PuzzleInventory.Instance == null) return;

            // 레버 획득 처리
            PuzzleInventory.Instance.AddLever(leverIndex);

            // 월드에서 레버 비활성화
            gameObject.SetActive(false);
        }

        #endregion
    }
}
