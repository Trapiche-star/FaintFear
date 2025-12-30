using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 레버 아이템 픽업 처리
    /// 퍼즐 인벤토리에 레버를 추가한다.
    /// </summary>
    public class LeverPickup : Interactive, IActionProvider
    {
        #region Variables

        // 이 레버의 번호 (0~3)
        [SerializeField]
        private int leverIndex;

        // ActionUI에 표시할 문구
        [SerializeField]
        private string actionText = "레버";

        #endregion


        #region Interactive Override

        // 플레이어가 E 키로 상호작용했을 때 호출된다
        public override void Interaction()
        {
            // 퍼즐 인벤토리가 없으면 더 이상 처리하지 않는다
            if (PuzzleInventory.Instance == null) return;

            // 이 레버 번호를 퍼즐 인벤토리에 추가한다
            PuzzleInventory.Instance.AddLever(leverIndex);

            // 레버를 획득했으므로 월드에서 비활성화한다
            gameObject.SetActive(false);
        }

        #endregion


        #region IActionProvider

        // ActionUI에 표시할 상호작용 문구를 반환한다
        public string GetActionText()
        {
            // ActionUI가 [E]를 자동으로 붙이므로
            // 여기서는 행동 대상 이름만 반환한다
            return actionText;
        }

        #endregion
    }
}
