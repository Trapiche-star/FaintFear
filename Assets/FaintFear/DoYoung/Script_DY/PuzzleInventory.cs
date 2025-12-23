using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 퍼즐 전용 인벤토리
    /// 소비형 퍼즐 아이템과 영구 퍼즐 도구의 보유 상태를 관리한다.
    /// </summary>
    public class PuzzleInventory : MonoBehaviour
    {
        #region Singleton

        // 퍼즐 인벤토리 싱글톤 인스턴스
        public static PuzzleInventory Instance;

        // 퍼즐 인벤토리 싱글톤 초기화
        private void Awake()
        {
            // 이미 인스턴스가 존재하고 그 객체가 자신이 아니라면
            if (Instance != null && Instance != this)
            {
                // 중복 생성된 인벤토리는 제거한다
                Destroy(gameObject);
                return;
            }

            // 이 객체를 싱글톤 인스턴스로 등록한다
            Instance = this;
        }

        #endregion


        #region Variables

        // 레버 퍼즐 슬롯 보유 여부 (소비형 퍼즐 아이템)
        private bool[] ownedLevers = new bool[4];

        // 볼트 커터 보유 여부 (영구 퍼즐 도구)
        private bool hasBoltCutter = false;

        #endregion


        #region Custom Method

        // 레버를 하나라도 가지고 있는지 확인한다
        public bool HasAnyLever()
        {
            // 모든 레버 슬롯을 순회하며 하나라도 true가 있는지 검사한다
            for (int i = 0; i < ownedLevers.Length; i++)
            {
                // 하나라도 보유 중이라면 true를 반환한다
                if (ownedLevers[i])
                    return true;
            }

            // 모든 슬롯이 false면 레버를 하나도 가지고 있지 않다
            return false;
        }

        // 특정 인덱스의 레버를 가지고 있는지 확인한다
        public bool HasLever(int leverIndex)
        {
            // 인덱스가 범위를 벗어나면 잘못된 접근이므로 false를 반환한다
            if (leverIndex < 0 || leverIndex >= ownedLevers.Length)
                return false;

            // 해당 인덱스의 레버 보유 상태를 그대로 반환한다
            return ownedLevers[leverIndex];
        }

        // 레버를 획득한다
        public void AddLever(int leverIndex)
        {
            // 인덱스가 유효하지 않으면 아무 처리도 하지 않는다
            if (leverIndex < 0 || leverIndex >= ownedLevers.Length)
                return;

            // 해당 인덱스의 레버를 보유 상태로 변경한다
            ownedLevers[leverIndex] = true;
        }

        // 레버를 소비한다
        public bool ConsumeLever(int leverIndex)
        {
            // 해당 레버를 가지고 있지 않다면 소비할 수 없다
            if (!HasLever(leverIndex))
                return false;

            // 레버를 소비했으므로 보유 상태를 해제한다
            ownedLevers[leverIndex] = false;

            // 정상적으로 소비되었음을 알린다
            return true;
        }

        // 볼트 커터를 획득한다
        public void AcquireBoltCutter()
        {
            // 이미 볼트 커터를 보유 중이라면 중복 획득을 방지한다
            if (hasBoltCutter)
                return;

            // 볼트 커터를 영구 퍼즐 도구로 등록한다
            hasBoltCutter = true;
        }

        #endregion


        #region Property

        // 볼트 커터 보유 여부를 반환한다
        public bool HasBoltCutter
        {
            get { return hasBoltCutter; }
        }

        #endregion
    }
}
