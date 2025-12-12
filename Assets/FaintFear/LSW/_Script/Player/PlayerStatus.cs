using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 플레이어 상태를 관리하는 싱글톤 클래스
    /// 체력, 배터리, 열쇠 보유 여부 등을 관리함
    /// </summary>
    public class PlayerStatus : Singleton<PlayerStatus>
    {
        [Header("Player Data")]
        public float maxMentalPower = 100f;
        public float currentMentalPower;

        [Header("Flashlight Battery")]
        public float maxBattery = 100f;   // 최대 배터리  
        public float currentBattery;      // 현재 배터리  

        [Header("Key / Inventory")]
        public bool hasKey = false;       // 열쇠 보유 여부 (true면 이미 획득한 상태)

        /// <summary>
        /// [최초 1회 실행]  
        /// 싱글톤이 처음 만들어질 때 딱 한 번 실행됨.
        /// </summary>
        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();

            // 초기 상태 세팅
            currentMentalPower = maxMentalPower;
            currentBattery = 0f;
            hasKey = false;
        }

        /// <summary>
        /// [씬 변경 시마다 실행]
        /// 씬이 로드될 때마다 실행됨.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        #region Public Methods
        // 체력 설정
        public void SetHealth(float value)
        {
            currentMentalPower = Mathf.Clamp(value, 0f, maxMentalPower);
        }

        // 배터리 충전
        public void AddBattery(float amount)
        {
            currentBattery += amount;
            if (currentBattery > maxBattery)
                currentBattery = maxBattery;
        }

        // 열쇠 획득
        public void AcquireKey()
        {
            hasKey = true;
            Debug.Log("플레이어가 열쇠를 획득했습니다.");
        }

        // 열쇠 상태 확인
        public bool HasKey()
        {
            return hasKey;
        }
        #endregion
    }
}
