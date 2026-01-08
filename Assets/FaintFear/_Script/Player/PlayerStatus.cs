using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace FaintFear
{
    public class PlayerStatus : Singleton<PlayerStatus>, ISaveableWorldObject
    {
        #region Variables

        public float maxMentalPower = 100f;
        public float currentMentalPower;
        public bool isMentalSystemActive = false;

        public float maxBattery = 100f;
        public float currentBattery;
        public bool isBatteryActive = false;
        public float BatteryNormalized => currentBattery / maxBattery;
        public int batteryCount;

        private HashSet<RoomKeyType> ownedKeys = new HashSet<RoomKeyType>();

        #endregion

        #region Unity Event Method

        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();

            // ⭐ 항상 기본값으로 초기화
            ResetStatus();

            Debug.Log("[PlayerStatus] 초기화 완료");
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        #endregion

        #region Custom Method

        // ===================== 정신력 =====================

        public void SetHealth(float value)
        {
            currentMentalPower = Mathf.Clamp(value, 0f, maxMentalPower);
        }

        // ===================== 배터리 =====================

        public void AddBattery(int amount = 1)
        {
            batteryCount += amount;

            if (currentBattery <= 0f)
                UseBattery();
        }

        public bool UseBattery()
        {
            if (batteryCount <= 0)
                return false;

            batteryCount--;
            currentBattery = maxBattery;

            return true;
        }

        // ===================== 열쇠 =====================

        public void AcquireKey(RoomKeyType key)
        {
            if (key == RoomKeyType.None)
                return;

            ownedKeys.Add(key);
            Debug.Log($"[PlayerStatus] 열쇠 획득: {key}");
        }

        public bool HasKey(RoomKeyType key)
        {
            if (key == RoomKeyType.None)
                return true;

            bool hasKey = ownedKeys.Contains(key);
            Debug.Log($"[PlayerStatus] 열쇠 확인: {key} = {hasKey}");
            return hasKey;
        }

        public bool ConsumeKey(RoomKeyType key)
        {
            if (key == RoomKeyType.None)
                return false;

            if (!ownedKeys.Contains(key))
                return false;

            ownedKeys.Remove(key);
            Debug.Log($"[PlayerStatus] 열쇠 소모: {key}");
            return true;
        }

        // ⭐ 수정: 완전 초기화
        public void ResetStatus()
        {
            // 정신력 초기화
            currentMentalPower = maxMentalPower;
            isMentalSystemActive = false;

            // 배터리 초기화
            currentBattery = 0f;
            batteryCount = 0;
            isBatteryActive = false;

            // 열쇠 초기화
            ownedKeys.Clear();

            Debug.Log("[PlayerStatus] 상태 완전 초기화 - " +
                     $"체력:{currentMentalPower}, 배터리:{currentBattery}, " +
                     $"배터리 개수:{batteryCount}, 열쇠 개수:{ownedKeys.Count}");
        }

        #endregion

        #region ISaveableWorldObject

        public string GetID() => "PlayerStatus_Global";

        public void Save(ref SaveData data)
        {
            // 열쇠를 문자열 리스트로 변환하여 저장
            data.ownedKeys.Clear();
            foreach (RoomKeyType key in ownedKeys)
            {
                data.ownedKeys.Add(key.ToString());
            }

            Debug.Log($"[PlayerStatus] 저장 - 열쇠: [{string.Join(", ", data.ownedKeys)}]");
        }

        public void Load(SaveData data)
        {
            // 저장된 열쇠 복원
            ownedKeys.Clear();

            if (data.ownedKeys != null)
            {
                foreach (string keyStr in data.ownedKeys)
                {
                    if (System.Enum.TryParse<RoomKeyType>(keyStr, out RoomKeyType key))
                    {
                        ownedKeys.Add(key);
                    }
                }
            }

            Debug.Log($"[PlayerStatus] 로드 - 열쇠: [{string.Join(", ", ownedKeys)}]");
        }

        #endregion
    }
}