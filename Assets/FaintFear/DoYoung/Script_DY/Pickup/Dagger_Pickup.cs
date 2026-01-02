using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 대거를 획득하는 퍼즐 전용 픽업 오브젝트
    /// 플레이어가 상호작용하면 대거 보유 상태를 등록하고 월드 오브젝트를 제거한다
    /// </summary>
    public class Dagger_Pickup : Interactive, IActionProvider
    {
        #region Variables

        [SerializeField] private string actionText = "대거를 줍는다"; // 액션 UI 문구

        private bool isPickedUp = false; // 이미 획득되었는지 여부

        #endregion


        #region Custom Method

        // 플레이어 상호작용 시 호출된다
        public override void Interaction()
        {
            if (isPickedUp)
                return; // 만약 [이미 획득된 상태라면] [중복 상호작용을 차단한다]

            if (PuzzleInventory.Instance == null)
                return; // 만약 [퍼즐 인벤토리가 존재하지 않는다면] [획득 처리를 중단한다]

            isPickedUp = true;
            // 중복 획득 방지를 위해 상태를 먼저 고정한다

            PuzzleInventory.Instance.AcquireBoltCutter();
            // 대거를 영구 퍼즐 도구로 등록한다

            Destroy(gameObject);
            // 월드에 존재하던 대거 오브젝트를 제거한다
        }

        #endregion


        #region Property

        // ActionUI에 표시될 상호작용 문구를 반환한다
        public string GetActionText()
        {
            return actionText;
        }

        #endregion
    }
}
