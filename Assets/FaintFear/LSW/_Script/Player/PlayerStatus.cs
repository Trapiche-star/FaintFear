using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace FaintFear
{
    public class PlayerStatus : Singleton<PlayerStatus>
    {
        [Header("Player Data")]
        public float maxMentalPower = 100f;
        public float currentMentalPower;


        //손전등
        public float maxBattery = 100f;       // 최대 배터리  
        public float currentBattery;          // 현재 배터리  

        /// <summary>
        /// [최초 1회 실행]
        /// 싱글톤이 처음 만들어질 때 딱 한 번 실행.
        /// </summary>
        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();

            // 체력 초기화
            currentMentalPower = maxMentalPower;
            currentBattery = 0f;
        }

        /// <summary>
        /// [씬 변경 시마다 실행]
        /// 씬이 로드될 때마다 실행.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public void SetHealth(float value)
        {
            currentMentalPower = value;
        }

        //배터리 충전
        public void AddBattery(float amount)
        {
            currentBattery += amount;

            //최대값 넘지 않게
            if (currentBattery > maxBattery)
                currentBattery = maxBattery;
        }
    }
}