using UnityEngine;
using System.Collections.Generic;

namespace FaintFear
{
    /// <summary>
    /// 플레이어의 핵심 상태를 관리하는 싱글톤 클래스
    /// 정신력(체력), 배터리, 열쇠(RoomKeyType) 시스템을 관리함
    /// </summary>
    public class PlayerStatus : Singleton<PlayerStatus>
    {
        #region Variables

        // ===================== 정신력 관련 =====================

        // 최대 정신력 값
        public float maxMentalPower = 100f;

        // 현재 정신력 값
        public float currentMentalPower;

        // 정신력 시스템 사용 여부 (연출/시스템 제어용)
        public bool isMentalSystemActive = false;

        // ===================== 배터리 관련 =====================

        // 최대 배터리 용량
        public float maxBattery = 100f;

        // 현재 배터리 잔량
        public float currentBattery;

        // 배터리 시스템 사용 여부
        public bool isBatteryActive = false;

        // UI에서 사용하기 위한 정규화된 배터리 값 (0~1)
        public float BatteryNormalized => currentBattery / maxBattery;

        // 플레이어가 소지 중인 배터리 개수
        public int batteryCount;

        // ===================== 열쇠 관련 =====================

        // 플레이어가 보유한 열쇠 목록 (중복 방지용)
        private HashSet<RoomKeyType> ownedKeys = new HashSet<RoomKeyType>();

        #endregion


        #region Unity Event Method

        // 싱글톤이 최초 생성될 때 단 한 번 실행
        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();

            // 정신력 초기화
            currentMentalPower = maxMentalPower;

            // 배터리 초기화
            currentBattery = 0f;
            batteryCount = 0;

            // 시스템 기본 비활성화
            isMentalSystemActive = false;
            isBatteryActive = false;

            // 열쇠 목록 초기화
            ownedKeys.Clear();

            if (!SaveSystem.HasSave())
            {
                ResetStatus(); 
            }
        }

        // 씬이 로드될 때마다 실행
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        #endregion


        #region Custom Method

        // ===================== 정신력 =====================

        // 정신력 값을 설정 (최소 0, 최대 maxMentalPower)
        public void SetHealth(float value)
        {
            currentMentalPower = Mathf.Clamp(value, 0f, maxMentalPower);
        }

        // ===================== 배터리 =====================

        // 배터리를 획득했을 때 호출
        public void AddBattery(int amount = 1)
        {
            // 배터리 개수 증가
            batteryCount += amount;

            // 배터리가 비어 있으면 즉시 하나 사용
            if (currentBattery <= 0f)
                UseBattery();
        }

        // 배터리를 실제로 사용하는 처리
        public bool UseBattery()
        {
            // 배터리가 없으면 사용 불가
            if (batteryCount <= 0)
                return false;

            // 배터리 하나 소모
            batteryCount--;

            // 배터리 잔량 최대치로 충전
            currentBattery = maxBattery;

            return true;
        }

        // ===================== 열쇠 =====================

        // 특정 타입의 열쇠를 획득
        public void AcquireKey(RoomKeyType key)
        {
            // None 타입은 무시
            if (key == RoomKeyType.None)
                return;

            // 열쇠 목록에 추가
            ownedKeys.Add(key);

            // 디버그 로그 출력
            Debug.Log($"플레이어가 [{key}] 열쇠를 획득했습니다.");
        }

        // 특정 열쇠를 가지고 있는지 확인
        public bool HasKey(RoomKeyType key)
        {
            // 열쇠가 필요 없는 경우 항상 true
            if (key == RoomKeyType.None)
                return true;

            // 보유 목록에 있는지 확인
            return ownedKeys.Contains(key);
        }

        // 특정 열쇠를 소모 (문/퍼즐에서 사용 시 호출)
        public bool ConsumeKey(RoomKeyType key)
        {
            // None 타입은 소모 개념이 없으므로 실패 처리
            if (key == RoomKeyType.None)
                return false;

            // 해당 열쇠를 보유하고 있지 않으면 소모 불가
            if (!ownedKeys.Contains(key))
                return false;

            // 열쇠를 보유 목록에서 제거하여 소모 처리
            ownedKeys.Remove(key);

            // 열쇠 소모가 정상적으로 완료되었음을 알림
            return true;
        }
        public void ResetStatus()
        {
            currentMentalPower = maxMentalPower;
            currentBattery = 0;
            batteryCount = 0;
        }
        #endregion
    }
}
