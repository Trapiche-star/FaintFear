using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 슬롯과 상호작용하는 트리거
    /// 메인 전력 공급 여부에 따라 슬롯 삽입을 제한한다
    /// </summary>
    public class SlotCollider : Interactive, IActionProvider
    {
        #region Variables

        [SerializeField] private SlotController slotController;
        // 슬롯 조건 확인과 실제 삽입 처리를 담당

        [SerializeField] private PowerBoxController powerBox;
        // 파워박스 전체 상태(전력 공급 여부)를 확인하기 위한 참조

        [SerializeField] private SequenceTextManager sequenceText;
        // HUD 텍스트 출력 도구

        [SerializeField] private string actionText;
        // ActionUI에 표시될 기본 상호작용 문구

        [SerializeField] private string needLeverMessage;
        // 레버가 없을 때 출력되는 안내 메시지

        [SerializeField] private string wrongLeverMessage;
        // 슬롯에 맞지 않을 때 출력되는 실패 메시지

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            if (sequenceText == null)
                sequenceText = Object.FindAnyObjectByType<SequenceTextManager>();
            // 만약 [SequenceTextManager가 연결되지 않았다면] [씬에 존재하는 인스턴스를 찾아 사용한다]
        }

        #endregion


        #region Interactive Override

        public override void Interaction()
        {
            if (slotController == null) return;
            // 만약 [슬롯 컨트롤러가 없다면] [상호작용을 중단한다]

            if (powerBox != null &&
                !powerBox.IsPowerSupplied &&
                slotController.RequiredLeverIndex != 0)
            {
                sequenceText?.ShowMessage(
                    "이 스위치는 메인 스위치가 활성화되어야 반응하는 것 같다."
                );
                return;
                // 만약 [메인 전력이 공급되지 않았고] [메인 슬롯이 아니라면] [시퀀스만 출력한다]
            }

            if (PuzzleInventory.Instance == null)
            {
                sequenceText?.ShowMessage(needLeverMessage);
                return;
                // 만약 [퍼즐 인벤토리가 없다면] [레버 필요 메시지를 출력한다]
            }

            if (!PuzzleInventory.Instance.HasAnyLever())
            {
                sequenceText?.ShowMessage(needLeverMessage);
                return;
                // 만약 [레버를 하나도 가지고 있지 않다면] [레버 필요 메시지를 출력한다]
            }

            if (!PuzzleInventory.Instance.HasLever(slotController.RequiredLeverIndex))
            {
                sequenceText?.ShowMessage(wrongLeverMessage);
                return;
                // 만약 [슬롯에 맞는 레버가 아니라면] [실패 메시지를 출력한다]
            }

            slotController.TryInsert();
            // 모든 조건이 충족되었으므로 슬롯 삽입을 시도한다
        }

        #endregion


        #region Property

        public string GetActionText()
        {
            if (slotController != null && slotController.IsFilled)
                return string.Empty;
            // 만약 [슬롯이 이미 채워져 있다면] [액션 문구를 숨긴다]

            return actionText;
        }

        #endregion
    }
}
