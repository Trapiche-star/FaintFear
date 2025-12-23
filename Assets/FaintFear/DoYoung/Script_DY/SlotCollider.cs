using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 슬롯과 상호작용하는 트리거
    /// 레버 보유 상태에 따라 HUD 메시지를 출력하거나 슬롯 삽입을 시도한다
    /// </summary>
    public class SlotCollider : Interactive, IActionProvider
    {
        #region Variables

        [SerializeField] private SlotController slotController; // 슬롯 조건 확인과 실제 삽입 처리를 담당
        [SerializeField] private SequenceTextManager sequenceText; // HUD 텍스트 출력 도구
        [SerializeField] private string actionText;              // ActionUI에 표시될 기본 상호작용 문구
        [SerializeField] private string needLeverMessage;         // 레버가 없을 때 출력되는 안내 메시지
        [SerializeField] private string wrongLeverMessage;        // 슬롯에 맞지 않을 때 출력되는 실패 메시지

        #endregion


        #region Unity Event Method

        // 씬 시작 시 sequenceText 참조를 확보한다
        private void Awake()
        {
            if (sequenceText == null)
                sequenceText = Object.FindAnyObjectByType<SequenceTextManager>();
            // 만약 SequenceTextManager가 연결되어 있지 않으면 씬에 존재하는 SequenceTextManager 찾아 사용한다
        }

        #endregion


        #region Interactive Override

        // 플레이어가 상호작용 키(E)를 눌렀을 때 호출된다
        public override void Interaction()
        {
            if (slotController == null) return;
            // 만약 슬롯 컨트롤러가 없으면 이 메서드에서는 더 이상 상호작용하지 않는다

            if (PuzzleInventory.Instance == null)
            {
                sequenceText?.ShowMessage(needLeverMessage);
                // 만약 퍼즐 인벤토리가 존재하지 않으면 HUD가 존재할 때 레버 필요 메시지를 출력한다
                return;
                // 만약 위 조건이 참이면 이 메서드에서는 더 이상 상호작용하지 않는다
            }

            if (!PuzzleInventory.Instance.HasAnyLever())
            {
                sequenceText?.ShowMessage(needLeverMessage);
                // 만약 레버를 하나도 가지고 있지 않으면 HUD가 존재할 때 레버 필요 메시지를 출력한다
                return;
                // 만약 위 조건이 참이면 이 메서드에서는 더 이상 상호작용하지 않는다
            }

            if (!PuzzleInventory.Instance.HasLever(slotController.RequiredLeverIndex))
            {
                sequenceText?.ShowMessage(wrongLeverMessage);
                // 만약 슬롯에 필요한 레버가 없으면 HUD가 존재할 때 슬롯 불일치 메시지를 출력한다
                return;
                // 만약 위 조건이 참이면 이 메서드에서는 더 이상 상호작용하지 않는다
            }

            slotController.TryInsert();
            // 모든 조건이 충족되었으므로 슬롯 삽입을 시도한다
        }

        #endregion


        #region Property

        // ActionUI에 표시할 상호작용 문구를 반환한다
        public string GetActionText()
        {
            if (slotController != null && slotController.IsFilled)
                return string.Empty;
            // 만약 슬롯이 이미 채워져 있다면 이 메서드에서는 문구를 표시하지 않는다

            return actionText;
            // 위 조건에 해당하지 않으면 기본 상호작용 문구를 반환한다
        }

        #endregion
    }
}
