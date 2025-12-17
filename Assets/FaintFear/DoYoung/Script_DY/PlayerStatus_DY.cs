using UnityEngine;
using System.Collections.Generic;

namespace FaintFear
{
    /// <summary>
    /// 플레이어의 핵심 상태를 관리하는 싱글톤 클래스 (개인 확장용)
    /// - 정신력
    /// - 손전등 배터리
    /// - 보유 열쇠 목록
    /// </summary>
    public class PlayerStatus_DY : Singleton<PlayerStatus_DY>
    {
        #region Variables

        [Header("Player Data")]
        public float maxMentalPower = 100f;     // 최대 정신력
        public float currentMentalPower;        // 현재 정신력

        [Header("Flashlight Battery")]
        public float maxBattery = 100f;         // 최대 배터리
        public float currentBattery;            // 현재 배터리

        [Header("Key / Inventory")]
        // 플레이어가 보유한 열쇠 목록 (중복 방지용)
        private HashSet<RoomKeyType> ownedKeys = new HashSet<RoomKeyType>();

        #endregion


        #region Unity Event Method

        /// <summary>
        /// 싱글톤 최초 생성 시 1회 실행
        /// </summary>
        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();

            // 정신력 초기화
            currentMentalPower = maxMentalPower;

            // 배터리 초기화
            currentBattery = 0f;

            // 보유 열쇠 목록 초기화
            ownedKeys.Clear();
        }

        #endregion


        #region Custom Method

        // 정신력 설정
        public void SetHealth(float value)
        {
            // 0 ~ 최대값 사이로 제한
            currentMentalPower = Mathf.Clamp(value, 0f, maxMentalPower);
        }

        // 배터리 충전
        public void AddBattery(float amount)
        {
            // 배터리 증가 후 최대값 제한
            currentBattery = Mathf.Clamp(currentBattery + amount, 0f, maxBattery);
        }

        // 열쇠 획득
        public void AcquireKey(RoomKeyType key)
        {
            // None이면 무시
            if (key == RoomKeyType.None)
                return;

            // 열쇠 추가 (이미 있으면 자동으로 무시됨)
            ownedKeys.Add(key);

            // 디버그 로그
            Debug.Log($"플레이어가 [{key}] 열쇠를 획득했습니다.");
        }

        // 특정 열쇠 보유 여부 확인
        public bool HasKey(RoomKeyType key)
        {
            // 열쇠가 필요 없는 경우 항상 true
            if (key == RoomKeyType.None)
                return true;

            // 보유 목록에 해당 키가 있는지 검사
            return ownedKeys.Contains(key);
        }

        #endregion
    }
}
