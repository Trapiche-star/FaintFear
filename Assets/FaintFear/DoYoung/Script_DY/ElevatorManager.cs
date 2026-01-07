using UnityEngine;

namespace FaintFear
{
    public class ElevatorManager : MonoBehaviour, ISaveableWorldObject
    {
        #region Variables

        public static ElevatorManager Instance { get; private set; }
        private bool isPowerSupplied = false;

        // ⭐ 이미 체크포인트로 저장됐는지 추적
        private bool wasSavedAsCheckpoint = false;

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Custom Method

        public void SupplyPower()
        {
            if (isPowerSupplied)
                return;

            isPowerSupplied = true;

            // ⭐ 런타임 상태 기록
            RuntimeStateManager.RecordElevatorState(isPowerSupplied);

            // ⭐ 엘리베이터 활성화 시 자동 저장 (최초 1회)
            if (!wasSavedAsCheckpoint)
            {
                wasSavedAsCheckpoint = true;
                AutoSaveManager.Instance?.RequestSave("elevator_powered");
                Debug.Log("[ElevatorManager] 전력 공급 - 자동저장 요청");
            }
        }

        public bool IsElevatorAvailable()
        {
            return isPowerSupplied;
        }

        #endregion

        // ⭐ ISaveableWorldObject 구현
        public string GetID() => "ElevatorManager";

        public void Save(ref SaveData data)
        {
            data.elevatorData.isPowerSupplied = isPowerSupplied;
        }

        public void Load(SaveData data)
        {
            isPowerSupplied = data.elevatorData.isPowerSupplied;
            wasSavedAsCheckpoint = isPowerSupplied; // 로드 시 이미 저장됨으로 표시
        }
    }
}
