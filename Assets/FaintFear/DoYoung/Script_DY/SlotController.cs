using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 슬롯 상태 컨트롤러
    /// 레버 요구 조건을 검사하고 성공 시 슬롯을 채운다
    /// </summary>
    public class SlotController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private int requiredLeverIndex = 0;      // 이 슬롯이 요구하는 레버 인덱스
        [SerializeField] private GameObject insertedLever;        // 슬롯에 꽂혔을 때 표시될 레버 오브젝트
        [SerializeField] private PowerBoxController powerBox;     // 파워박스 퍼즐 관리자
        [SerializeField] private EndingManager endingManager;     // 엔딩 조건 판별 매니저 (알림용)
        [SerializeField] private ElevatorManager elevatorManager; // 엘리베이터 전력 매니저 (알림용)

        private bool isFilled = false;                             // 슬롯이 이미 채워졌는지 여부

        #endregion


        #region Property

        public bool IsFilled => isFilled;                          // 슬롯 채워짐 상태 반환
        public int RequiredLeverIndex => requiredLeverIndex;      // 요구 레버 인덱스 반환

        #endregion


        #region Custom Method

        // 슬롯에 레버를 삽입하려고 시도한다
        public bool TryInsert()
        {
            if (isFilled)
                return false; // 만약 [이미 슬롯이 채워져 있다면] [삽입을 허용하지 않는다]

            if (PuzzleInventory.Instance == null)
                return false; // 만약 [퍼즐 인벤토리가 없다면] [처리를 중단한다]

            if (!PuzzleInventory.Instance.HasLever(requiredLeverIndex))
                return false; // 만약 [요구 레버를 소지하지 않았다면] [삽입에 실패한다]

            PuzzleInventory.Instance.ConsumeLever(requiredLeverIndex);
            // 레버를 소모한다

            FillSlot();
            // 슬롯을 채우는 실제 처리를 실행한다

            return true;
        }

        // 슬롯을 채웠을 때의 실제 처리
        private void FillSlot()
        {
            if (isFilled)
                return; // 만약 [이미 채워진 상태라면] [중복 실행을 방지한다]

            isFilled = true;
            // 슬롯 상태를 채워진 상태로 변경한다

            if (insertedLever != null)
                insertedLever.SetActive(true);
            // 슬롯에 맞는 레버 오브젝트를 화면에 표시한다

            if (endingManager != null)
                endingManager.SetLeverActivated(requiredLeverIndex);
            // 이 슬롯의 레버가 활성화되었음을 엔딩 매니저에 알린다

            if (elevatorManager != null && requiredLeverIndex == 0)
                elevatorManager.SupplyPower();
            // 만약 [이 슬롯이 빨간 스위치라면] [엘리베이터 전력을 공급한다]

            if (powerBox != null)
                powerBox.CheckPuzzleComplete();
            // 퍼즐 전체 완료 여부를 다시 검사하도록 보고한다
        }

        #endregion
    }
}
