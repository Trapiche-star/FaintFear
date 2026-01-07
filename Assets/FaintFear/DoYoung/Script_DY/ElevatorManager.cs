using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 엘리베이터 전력 상태 매니저
    /// 빨간 스위치 활성화 여부를 저장하고 엘리베이터 오픈 가능 상태를 관리한다
    /// 전역 싱글톤으로 유지되어 씬 이동과 무관하게 상태를 보존한다
    /// </summary>
    public class ElevatorManager : MonoBehaviour
    {
        #region Variables

        public static ElevatorManager Instance { get; private set; }
        // 전역 접근용 싱글톤 인스턴스

        private bool isPowerSupplied = false;
        // 엘리베이터 전력 공급 여부

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
                // 만약 [이미 인스턴스가 존재한다면] [중복 객체를 제거한다]
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 씬 이동 시에도 이 객체를 유지한다
        }

        #endregion


        #region Custom Method

        // 빨간 스위치가 활성화되었음을 전달받는다
        public void SupplyPower()
        {
            if (isPowerSupplied)
                return; // 만약 [이미 전력이 공급된 상태라면] [중복 처리를 하지 않는다]

            isPowerSupplied = true;
            // 엘리베이터 전력이 공급되었음을 기록한다
        }

        // 엘리베이터 사용 가능 여부를 반환한다
        public bool IsElevatorAvailable()
        {
            return isPowerSupplied;
            // 전력이 공급된 경우에만 엘리베이터 사용이 가능하다
        }

        #endregion
    }
}
