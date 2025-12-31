using UnityEngine;
using System;
using System.Collections;

namespace FaintFear
{
    public class AutoSaveManager : Singleton<AutoSaveManager>
    {
        [Header("Save Condition")]
        public float requiredMental = 50f;

        bool isEnemyChasing = false;   // EnemyManager에서 세팅
        bool pendingSave = false;
        string pendingCheckpointId;
        public static event Action OnAutoSaveStart;
        public static event Action OnAutoSaveEnd;

        // 싱글톤이 최초 생성될 때 단 한 번 실행
        protected override void OnPreInitialize()
        {

        }

        // 씬이 로드될 때마다 실행
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        public void RequestSave(string checkpointId)
        {
            if (CanSave())
            {
                StartCoroutine(SaveRoutine());
            }
            else
            {
                // 조건 불만족 → 보류
                pendingSave = true;
                pendingCheckpointId = checkpointId;
            }
        }

        void Update()
        {
            // 보류된 저장 처리
            if (pendingSave && CanSave())
            {
                pendingSave = false;
                StartCoroutine(SaveRoutine());
            }
        }

        bool CanSave()
        {
            return !isEnemyChasing &&
                   PlayerStatus.Instance.currentMentalPower >= requiredMental;
        }

        IEnumerator SaveRoutine()
        {
            OnAutoSaveStart?.Invoke(); // ✅ UI 있으면 반응

            yield return new WaitForSeconds(2.0f);

            SaveSystem.SaveGame(pendingCheckpointId);

            OnAutoSaveEnd?.Invoke();   // ✅
        }

        // 외부에서 호출
        public void SetEnemyChasing(bool value)
        {
            isEnemyChasing = value;
        }
        
    }
}
