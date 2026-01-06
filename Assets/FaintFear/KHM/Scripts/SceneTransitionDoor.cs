using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 씬 이동 전용 문 (디버깅 로그 추가)
    /// </summary>
    public class SceneTransitionDoor : Interactive, IActionProvider
    {
        #region Variables
        [Header("Lock State")]
        [SerializeField] private bool isLocked = true;

        [Header("Scene Settings")]
        [SerializeField] private string targetSceneName = "BasementScene";
        [SerializeField] private string spawnPointName = "FromBasement";

        [Header("Messages")]
        [SerializeField] private SequenceTextManager sequenceText;

        [Header("Custom Messages")]
        [SerializeField, TextArea] private string lockedMessage = "문이 잠겨있다. 키패드로 열 수 있을 것 같다.";
        [SerializeField, TextArea] private string transitionMessage = "문을 열고 들어간다...";

        private bool isTransitioning = false;
        #endregion

        #region Interactive Override

        public override void Interaction()
        {
            if (isTransitioning) return;

            if (isLocked)
            {
                ShowMessage(lockedMessage);
                return;
            }

            StartSceneTransition();
        }

        #endregion

        #region Public Methods

        public void Unlock()
        {
            isLocked = false;
            ShowMessage("문의 잠금이 해제되었다.");
        }

        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }

        public bool IsLocked()
        {
            return isLocked;
        }

        #endregion

        #region Private Methods

        private void StartSceneTransition()
        {
            isTransitioning = true;


            ShowMessage(transitionMessage);

            if (SceneLoadManager.Instance != null)
            {
                SceneLoadManager.Instance.LoadScene(targetSceneName, spawnPointName);
            }
            else
            {
                Debug.LogError("[SceneTransitionDoor] SceneLoadManager not found!");
            }
        }

        private void ShowMessage(string message)
        {
            if (sequenceText != null && !string.IsNullOrEmpty(message))
            {
                sequenceText.ShowMessage(message);
            }
        }

        #endregion

        #region IActionProvider Implementation

        public string GetActionText()
        {
            if (isTransitioning)
                return string.Empty;

            return "문 열기";
        }

        #endregion
    }
}