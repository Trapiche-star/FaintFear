using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// PowerBox 퍼즐 전체 관리자
    /// 슬롯 상태를 종합하여 퍼즐 완료를 판단하고
    /// 슬롯 상호작용 가능 여부를 제어한다.
    /// </summary>
    public class PowerBoxController : MonoBehaviour
    {
        #region Variables

        // 파워박스에 포함된 슬롯 컨트롤러들
        [SerializeField]
        private SlotController[] slots;

        // 각 슬롯의 상호작용 스크립트들
        [SerializeField]
        private SlotCollider[] slotColliders;

        // 퍼즐 완료 여부
        private bool isCompleted = false;

        #endregion


        #region Public Methods

        // 슬롯 상태 변경 시 호출되어 퍼즐 완료 여부를 검사한다
        public void CheckPuzzleComplete()
        {
            // 이미 퍼즐이 완료된 상태면 더 이상 검사하지 않는다
            if (isCompleted) return;

            // 모든 슬롯이 채워졌는지 확인한다
            for (int i = 0; i < slots.Length; i++)
            {
                // 하나라도 채워지지 않았다면 완료 아님
                if (!slots[i].IsFilled)
                    return;
            }

            // 모든 슬롯이 채워졌다면 퍼즐 완료 처리로 넘어간다
            OnPuzzleCompleted();
        }

        // 슬롯 상호작용 가능 여부를 전체 제어한다
        public void SetSlotTriggerActive(bool isActive)
        {
            // 모든 슬롯 상호작용 스크립트를 순회한다
            for (int i = 0; i < slotColliders.Length; i++)
            {
                // 슬롯 콜라이더가 존재할 때만 처리한다
                if (slotColliders[i] != null)
                    slotColliders[i].enabled = isActive; // 상호작용만 켜거나 끈다
            }
        }

        #endregion


        #region Private Methods

        // 퍼즐 완료 시 한 번만 호출된다
        private void OnPuzzleCompleted()
        {
            // 퍼즐을 완료 상태로 설정한다
            isCompleted = true;

            // 슬롯 상호작용을 더 이상 허용하지 않는다
            SetSlotTriggerActive(false);

            // 퍼즐 완료 로그 (나중에 전기 ON 연출로 교체)
            Debug.Log("PowerBox 퍼즐 완료");
        }

        #endregion
    }
}
