using UnityEngine;

namespace FaintFear
{
    public class EndingManager : MonoBehaviour, ISaveableWorldObject
    {
        #region Variables

        public static EndingManager Instance { get; set; }

        private bool[] activatedLevers = new bool[4];

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

            // ⭐ Awake에서는 저장된 상태 복원하지 않음 (GameManager가 제어)
        }

        #endregion

        #region Custom Method

        public void SetLeverActivated(int leverIndex)
        {
            if (leverIndex < 0 || leverIndex >= activatedLevers.Length)
                return;

            activatedLevers[leverIndex] = true;
            RecordEndingState();

            Debug.Log($"[EndingManager] 레버 {leverIndex} 활성화");
        }

        public bool CanEnterEndingA()
        {
            bool allActivated =
                activatedLevers[0] &&
                activatedLevers[1] &&
                activatedLevers[2] &&
                activatedLevers[3];

            if (allActivated)
                return false;

            return activatedLevers[0];
        }

        public bool CanEnterEndingB()
        {
            return
                activatedLevers[0] &&
                activatedLevers[1] &&
                activatedLevers[2] &&
                activatedLevers[3];
        }

        // ⭐ 추가: 상태 초기화
        public void ResetState()
        {
            for (int i = 0; i < activatedLevers.Length; i++)
            {
                activatedLevers[i] = false;
            }

            Debug.Log("[EndingManager] 상태 초기화 완료");
        }

        private void RecordEndingState()
        {
            RuntimeStateManager.RecordEndingState((bool[])activatedLevers.Clone());
        }

        // ⭐ 추가: 저장된 상태 복원 (Continue 시에만 호출)
        public void RestoreLeverStates(SaveData data)
        {
            if (data == null || data.endingData == null)
            {
                Debug.LogWarning("[EndingManager] 복원할 데이터 없음");
                return;
            }

            if (data.endingData.activatedLevers == null || data.endingData.activatedLevers.Length == 0)
            {
                Debug.LogWarning("[EndingManager] activatedLevers가 비어있음");
                return;
            }

            for (int i = 0; i < activatedLevers.Length && i < data.endingData.activatedLevers.Length; i++)
            {
                activatedLevers[i] = data.endingData.activatedLevers[i];
            }

            Debug.Log($"[EndingManager] 레버 상태 복원: " +
                $"빨강={activatedLevers[0]}, 노랑={activatedLevers[1]}, " +
                $"검정={activatedLevers[2]}, 파랑={activatedLevers[3]}");
        }

        public string GetCurrentStatus()
        {
            return $"레버 상태 - 빨강:{activatedLevers[0]}, 노랑:{activatedLevers[1]}, " +
                   $"검정:{activatedLevers[2]}, 파랑:{activatedLevers[3]} | " +
                   $"엔딩A 가능:{CanEnterEndingA()}, 엔딩B 가능:{CanEnterEndingB()}";
        }

        #endregion

        #region ISaveableWorldObject

        public string GetID() => "EndingManager_Global";

        public void Save(ref SaveData data)
        {
            data.endingData.activatedLevers = (bool[])activatedLevers.Clone();
        }

        public void Load(SaveData data)
        {
            RestoreLeverStates(data);
        }

        #endregion
    }
}