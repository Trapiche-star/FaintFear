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

        // 퍼즐 인벤토리 싱글톤
        public static PuzzleInventory Instance;

        private void Awake()
        {
            // 중복 생성 방지
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        #endregion


        #region Variables

        // 레버 보유 여부 (인덱스 기반)
        private bool[] ownedLevers = new bool[4];

        #endregion


        #region Public Methods

        // 특정 레버를 보유하고 있는지 확인
        public bool HasLever(int leverIndex)
        {
            // 인덱스 범위 체크
            if (leverIndex < 0 || leverIndex >= ownedLevers.Length)
                return false;

            return ownedLevers[leverIndex];
        }

        // 레버 획득 처리
        public void AddLever(int leverIndex)
        {
            // 인덱스 범위 체크
            if (leverIndex < 0 || leverIndex >= ownedLevers.Length)
                return;

            ownedLevers[leverIndex] = true;
        }

        // 레버 소비 처리
        public bool ConsumeLever(int leverIndex)
        {
            // 보유하지 않았으면 실패
            if (!HasLever(leverIndex))
                return false;

            ownedLevers[leverIndex] = false;
            return true;
        }

        #endregion
    }
}
