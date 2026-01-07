using UnityEngine;
using System.Collections;

namespace FaintFear
{
    public class ElevatorOpen : Interactive, IActionProvider
    {
        #region Variables

        [Header("Reference")]
        [SerializeField] private SequenceTextManager textManager;

        [Header("Fail Lines")]
        [SerializeField] private string fail_First;
        [SerializeField] private string fail_Second;
        [SerializeField] private float lineHoldTime = 2.5f;

        [Header("Scene Settings")]
        [SerializeField] private string targetSceneName = "BasementScene";
        [SerializeField] private string spawnPointName = "FromBasement";

        [Header("Action Text")]
        [SerializeField] private string actionText = "사용하기";

        private int failCount = 0;
        private bool isLocked = false;

        #endregion

        #region Custom Method

        public override void Interaction()
        {
            if (isLocked)
                return;

            if (ElevatorManager.Instance == null)
                return;

            if (!ElevatorManager.Instance.IsElevatorAvailable())
            {
                HandleFailSequence();
                return;
            }

            ExecuteElevator();
        }

        private void HandleFailSequence()
        {
            failCount++;
            int index = failCount % 2;

            if (index == 1)
                PlayFailSequence(fail_First);
            else
                PlayFailSequence(fail_Second);
        }

        private void PlayFailSequence(string message)
        {
            if (textManager == null)
                return;

            StartCoroutine(PlayAndHide(message));
        }

        private void ExecuteElevator()
        {
            // ⭐ 엘리베이터 사용 시마다 자동 저장
            if (AutoSaveManager.Instance != null)
            {
                AutoSaveManager.Instance.RequestSave("elevator_used");
                Debug.Log("[ElevatorOpen] 엘리베이터 사용 - 자동저장 요청");
            }

            // + 엘리베이터 SFX 재생
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX("SFX_Elevator");
                Debug.Log("[ElevatorOpen] 엘리베이터 SFX 재생");
            }

            if (SceneLoadManager.Instance != null)
            {
                SceneLoadManager.Instance.LoadScene(targetSceneName, spawnPointName);
            }
            else
            {
                Debug.LogError("[ElevatorOpen] SceneLoadManager not found!");
            }

            Debug.Log("[ElevatorOpen] 엘리베이터 실행");
        }

        private IEnumerator PlayAndHide(string message)
        {
            isLocked = true;

            yield return StartCoroutine(
                textManager.ShowDialogueSequence(
                    new string[] { message },
                    lineHoldTime
                )
            );

            textManager.Hide();
            isLocked = false;
        }

        #endregion

        #region Property

        public string GetActionText()
        {
            return actionText;
        }

        #endregion
    }
}
