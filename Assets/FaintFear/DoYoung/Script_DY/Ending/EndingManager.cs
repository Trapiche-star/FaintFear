using UnityEngine;

namespace FaintFear
{
    public class EndingManager : MonoBehaviour, ISaveableWorldObject
    {
        #region Variables

        public static EndingManager Instance { get; private set; }

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

            // ⭐ 추가: 시작 시 저장된 상태 복원
            RestoreLeverStates();
        }

        #endregion

        #region Custom Method

        public void SetLeverActivated(int leverIndex)
        {
            if (leverIndex < 0 || leverIndex >= activatedLevers.Length)
                return;

            activatedLevers[leverIndex] = true;

            // ⭐ 추가: 레버 활성화 시 런타임 상태 기록
            RecordEndingState();
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

        // ⭐ 추가: 런타임 상태 기록
        private void RecordEndingState()
        {
            RuntimeStateManager.RecordEndingState(activatedLevers);
        }

        // ⭐ 추가: 저장된 상태 복원
        private void RestoreLeverStates()
        {
            SaveData data = SaveSystem.LoadPreview();
            if (data == null) return;

            // SaveData에서 레버 상태 복원
            if (data.endingData != null && data.endingData.activatedLevers != null)
            {
                for (int i = 0; i < activatedLevers.Length && i < data.endingData.activatedLevers.Length; i++)
                {
                    activatedLevers[i] = data.endingData.activatedLevers[i];
                }

                Debug.Log($"[EndingManager] 레버 상태 복원: " +
                    $"빨강={activatedLevers[0]}, 노랑={activatedLevers[1]}, " +
                    $"검정={activatedLevers[2]}, 파랑={activatedLevers[3]}");
            }
        }

        // ⭐ 추가: 디버그용 - 현재 상태 확인
        public string GetCurrentStatus()
        {
            return $"레버 상태 - 빨강:{activatedLevers[0]}, 노랑:{activatedLevers[1]}, " +
                   $"검정:{activatedLevers[2]}, 파랑:{activatedLevers[3]} | " +
                   $"엔딩A 가능:{CanEnterEndingA()}, 엔딩B 가능:{CanEnterEndingB()}";
        }
        public string GetID() => "EndingManager_Global";

        public void Save(ref SaveData data)
        {
            data.endingData.activatedLevers = (bool[])activatedLevers.Clone();
        }

        public void Load(SaveData data)
        {
            if (data.endingData != null && data.endingData.activatedLevers != null)
            {
                for (int i = 0; i < activatedLevers.Length && i < data.endingData.activatedLevers.Length; i++)
                {
                    activatedLevers[i] = data.endingData.activatedLevers[i];
                }
            }
        }
        #endregion
    }
}