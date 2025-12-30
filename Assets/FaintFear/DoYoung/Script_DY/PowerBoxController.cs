using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// PowerBox 퍼즐 전체 관리자
    /// 슬롯 상태를 종합하여 퍼즐 완료를 판단하고
    /// 전력 공급 단계에 따라 슬롯 상호작용을 제어한다.
    /// </summary>
    public class PowerBoxController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private SlotController[] slots;
        // 파워박스에 포함된 슬롯 컨트롤러들

        [SerializeField] private SlotCollider[] slotColliders;
        // 각 슬롯의 상호작용 스크립트들

        [SerializeField] private SequenceTextManager sequenceText;
        // 전력 공급 시 시퀀스 텍스트 출력 담당

        private bool isCompleted = false;
        // 퍼즐 전체 완료 여부

        private bool isPowerSupplied = false;
        // 메인 전력이 공급되었는지 여부

        #endregion


        #region Unity Event Method

        private void Start()
        {
            SetSlotTriggerActive(false);
            // 퍼즐 시작 시 모든 슬롯 상호작용을 비활성화한다

            if (slotColliders.Length > 0)
                slotColliders[0].enabled = true;
            // 첫 번째 슬롯만 메인 전력 슬롯으로 활성화한다
        }

        #endregion


        #region Public Methods

        // 슬롯 상태 변경 시 호출되어 퍼즐 흐름과 완료 여부를 검사한다
        public void CheckPuzzleComplete()
        {
            if (slots.Length == 0) return;
            // 만약 슬롯 배열이 비어 있다면 더 이상 처리하지 않는다

            if (!isPowerSupplied && slots[0].IsFilled)
            {
                SupplyPower();
                // 만약 [메인 전력이 아직 공급되지 않았고] [첫 번째 슬롯이 채워졌다면] [전력 공급 처리를 실행한다]
            }

            if (isCompleted) return;
            // 만약 [퍼즐이 이미 완료된 상태라면] [더 이상 완료 검사를 하지 않는다]

            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsFilled)
                    return;
                // 만약 [하나라도 채워지지 않은 슬롯이 있다면] [퍼즐은 아직 완료되지 않았다]
            }

            OnPuzzleCompleted();
            // 모든 슬롯이 채워졌다면 퍼즐 완료 처리로 넘어간다
        }

        // 슬롯 상호작용 가능 여부를 전체 제어한다
        public void SetSlotTriggerActive(bool isActive)
        {
            for (int i = 0; i < slotColliders.Length; i++)
            {
                if (slotColliders[i] != null)
                    slotColliders[i].enabled = isActive;
                // 슬롯 콜라이더가 존재할 경우 상호작용 가능 여부를 설정한다
            }
        }

        #endregion


        #region Private Methods

        // 메인 전력이 공급되었을 때 한 번만 호출된다
        private void SupplyPower()
        {
            isPowerSupplied = true;
            // 메인 전력이 공급되었음을 기록한다

            for (int i = 1; i < slotColliders.Length; i++)
            {
                if (slotColliders[i] != null)
                    slotColliders[i].enabled = true;
                // 첫 번째 슬롯을 제외한 나머지 슬롯 상호작용을 활성화한다
            }

            if (sequenceText != null)
                sequenceText.ShowMessage("일부 시설들에 전력이 들어온 것 같다.");
            // 전력 공급 시 시퀀스 메시지를 출력한다
        }

        // 퍼즐 완료 시 한 번만 호출된다
        private void OnPuzzleCompleted()
        {
            isCompleted = true;
            // 퍼즐을 완료 상태로 설정한다

            SetSlotTriggerActive(false);
            // 모든 슬롯 상호작용을 비활성화한다

            Debug.Log("PowerBox 퍼즐 완료");
            // 퍼즐 완료 로그를 출력한다
        }

        #endregion
    }
}
