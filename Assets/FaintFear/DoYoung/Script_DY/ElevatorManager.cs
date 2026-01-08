using UnityEngine;

namespace FaintFear
{
    public class ElevatorManager : MonoBehaviour, ISaveableWorldObject
    {
        #region Variables

        public static ElevatorManager Instance { get; set; }

        private bool isPowerSupplied = false;
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

        // ⭐ 추가: 상태 초기화 (NewGame용)
        public void ResetState()
        {
            isPowerSupplied = false;
            wasSavedAsCheckpoint = false;

            Debug.Log("[ElevatorManager] 상태 초기화 완료");
        }

        #endregion

        #region ISaveableWorldObject

        public string GetID() => "ElevatorManager";

        public void Save(ref SaveData data)
        {
            data.elevatorData.isPowerSupplied = isPowerSupplied;

            Debug.Log($"[ElevatorManager] 저장 - 전력 공급: {isPowerSupplied}");
        }

        public void Load(SaveData data)
        {
            // ⭐ null 체크 추가
            if (data.elevatorData == null)
            {
                Debug.LogWarning("[ElevatorManager] elevatorData가 null입니다. 기본값 사용");
                return;
            }

            isPowerSupplied = data.elevatorData.isPowerSupplied;
            wasSavedAsCheckpoint = isPowerSupplied; // 로드 시 이미 저장됨으로 표시

            Debug.Log($"[ElevatorManager] 로드 - 전력 공급: {isPowerSupplied}");
        }

        #endregion
    }
}