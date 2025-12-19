using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 슬롯 콜라이더 상호작용
    /// ActionUI 문구를 제공하고 E 입력 시 슬롯 삽입을 시도한다.
    /// 실패 시 시퀀스 UI를 통해 메시지를 출력한다.
    /// </summary>
    public class SlotCollider : Interactive, IActionProvider
    {
        #region Variables

        // 연결된 슬롯 컨트롤러
        [SerializeField]
        private SlotController slotController;

        // 기본 상호작용 문구 (ActionUI용)
        [SerializeField]
        private string actionText = "작동";

        // 레버가 하나도 없을 때 표시할 메시지
        [SerializeField]
        private string needLeverMessage = "레버가 필요하다";

        // 레버는 있지만 슬롯에 맞지 않을 때 표시할 메시지
        [SerializeField]
        private string wrongLeverMessage = "이 슬롯에 맞지 않는다";

        // 시퀀스 텍스트 매니저
        [SerializeField]
        private SequenceTextManager sequenceText;

        #endregion


        #region Interactive Override

        // 플레이어가 E 키를 눌렀을 때 호출된다
        public override void Interaction()
        {
            // 슬롯 컨트롤러가 없으면 아무 처리도 하지 않는다
            if (slotController == null) return;

            // 퍼즐 인벤토리가 없으면 시도 자체를 중단한다
            if (PuzzleInventory.Instance == null) return;

            // 레버를 하나도 들고 있지 않으면
            if (!PuzzleInventory.Instance.HasAnyLever())
            {
                // 시퀀스 UI에 레버 필요 메시지를 표시한다
                if (sequenceText != null)
                    sequenceText.ShowMessage(needLeverMessage);

                return;
            }

            // 이 슬롯에 맞는 레버를 들고 있지 않으면
            if (!PuzzleInventory.Instance.HasLever(slotController.RequiredLeverIndex))
            {
                // 시퀀스 UI에 슬롯 불일치 메시지를 표시한다
                if (sequenceText != null)
                    sequenceText.ShowMessage(wrongLeverMessage);

                return;
            }

            // 여기까지 왔다는 것은 조건이 모두 맞다는 뜻이다
            // 실제 슬롯 삽입을 시도한다
            slotController.TryInsert();
        }

        #endregion


        #region IActionProvider

        // ActionUI에 표시할 문구를 반환한다
        public string GetActionText()
        {
            // 슬롯이 이미 채워졌다면 문구를 숨긴다
            if (slotController != null && slotController.IsFilled)
                return string.Empty;

            // 가능한 행동만 표시한다
            return actionText;
        }

        #endregion
    }
}
