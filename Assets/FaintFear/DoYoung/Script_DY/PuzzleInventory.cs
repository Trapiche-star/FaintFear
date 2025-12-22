using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 퍼즐 전용 인벤토리
    /// 레버, 카드 등 퍼즐에 사용되는 아이템의 보유 상태를 관리한다.
    /// </summary>
    public class PuzzleInventory : MonoBehaviour
    {
        #region Singleton

        // 퍼즐 인벤토리 싱글톤 인스턴스
        public static PuzzleInventory Instance;

        private void Awake()
        {
            // 이미 인스턴스가 있고, 그게 내가 아니면
            if (Instance != null && Instance != this)
            {
                // 중복 생성된 오브젝트는 제거한다
                Destroy(gameObject);
                return;
            }

            // 이 오브젝트를 싱글톤으로 등록한다
            Instance = this;
        }

        #endregion


        #region Variables

        // 레버 보유 여부를 인덱스로 관리한다 (0~3)
        private bool[] ownedLevers = new bool[4];

        #endregion


        #region Public Methods

        // 레버를 하나라도 가지고 있는지 확인한다
        public bool HasAnyLever()
        {
            // 모든 레버 슬롯을 하나씩 확인한다
            for (int i = 0; i < ownedLevers.Length; i++)
            {
                // 하나라도 true면 레버를 가지고 있는 상태다
                if (ownedLevers[i])
                    return true;
            }

            // 전부 false면 레버를 하나도 가지고 있지 않다
            return false;
        }

        // 특정 인덱스의 레버를 가지고 있는지 확인한다
        public bool HasLever(int leverIndex)
        {
            // 인덱스가 0보다 작거나 배열 크기보다 크면
            if (leverIndex < 0 || leverIndex >= ownedLevers.Length)
                return false; // 잘못된 접근이므로 false를 반환한다

            // 해당 인덱스의 보유 상태를 그대로 반환한다
            return ownedLevers[leverIndex];
        }

        // 레버를 획득한다
        public void AddLever(int leverIndex)
        {
            // 인덱스가 범위를 벗어나면
            if (leverIndex < 0 || leverIndex >= ownedLevers.Length)
                return; // 아무 처리도 하지 않는다

            // 해당 인덱스의 레버를 보유 상태로 바꾼다
            ownedLevers[leverIndex] = true;
        }

        // 레버를 소비한다
        public bool ConsumeLever(int leverIndex)
        {
            // 해당 레버를 가지고 있지 않으면
            if (!HasLever(leverIndex))
                return false; // 소비할 수 없으므로 실패 처리

            // 레버를 소비했으니 보유 상태를 false로 바꾼다
            ownedLevers[leverIndex] = false;

            // 정상적으로 소비했음을 알린다
            return true;
        }

        #endregion
    }
}
