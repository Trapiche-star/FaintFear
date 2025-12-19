using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// PowerBox 퍼즐 전체 관리자
    /// 슬롯 상태를 종합하여 퍼즐 완료를 판단하고
    /// 슬롯 트리거 콜라이더 활성/비활성을 제어한다.
    /// </summary>
    public class PowerBoxController : MonoBehaviour
    {
        #region Variables

        // 파워박스에 포함된 슬롯 컨트롤러들 (총 4개)
        [SerializeField]
        private SlotController[] slots;

        // 각 슬롯의 상호작용 트리거 콜라이더들
        [SerializeField]
        private Collider[] slotTriggerColliders;

        // 퍼즐 완료 여부
        private bool isCompleted = false;

        #endregion


        #region Public Methods

        // 슬롯 상태 변경 시 호출되어 퍼즐 완료 여부를 검사
        public void CheckPuzzleComplete()
        {
            // 이미 퍼즐이 완료된 경우 재검사하지 않음
            if (isCompleted) return;

            // 모든 슬롯이 채워졌는지 검사
            for (int i = 0; i < slots.Length; i++)
            {
                // 하나라도 비어 있으면 완료 아님
                if (!slots[i].IsFilled)
                    return;
            }

            // 모든 슬롯이 채워졌다면 퍼즐 완료 처리
            OnPuzzleCompleted();
        }

        // 슬롯 트리거 콜라이더 전체 활성/비활성 제어
        public void SetSlotTriggerActive(bool isActive)
        {
            // 모든 슬롯 트리거 콜라이더 순회
            for (int i = 0; i < slotTriggerColliders.Length; i++)
            {
                // 콜라이더가 존재할 때만 처리
                if (slotTriggerColliders[i] != null)
                    slotTriggerColliders[i].enabled = isActive;
            }
        }

        #endregion


        #region Private Methods

        // 퍼즐 완료 시 한 번만 호출
        private void OnPuzzleCompleted()
        {
            // 퍼즐 완료 상태로 전환
            isCompleted = true;

            // 슬롯 트리거를 더 이상 사용하지 않도록 비활성화
            SetSlotTriggerActive(false);

            // 퍼즐 완료 로그 (나중에 전기 ON 등으로 교체)
            Debug.Log("PowerBox 퍼즐 완료");
        }

        #endregion
    }
}
