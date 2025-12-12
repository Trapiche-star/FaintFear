using UnityEngine;

namespace FaintFear
{
    public class PlayerStatus : Singleton<PlayerStatus>
    {
        [Header("Player Data")]
        public float maxMentalPower = 100f;
        public float currentMentalPower;

        /// <summary>
        /// [최초 1회 실행]
        /// 싱글톤이 처음 만들어질 때 딱 한 번 실행.
        /// </summary>
        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();

            // 체력 초기화
            currentMentalPower = maxMentalPower;
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


    }
}