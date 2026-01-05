using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 씬 이동 전용 문
    /// 잠금 해제 후 상호작용 시 바로 씬 이동
    /// </summary>
    public class SceneTransitionDoor : Interactive, IActionProvider
    {
        #region Variables

        [Header("Lock State")]
        [SerializeField] private bool isLocked = true;

        [Header("Scene Settings")]
        [SerializeField] private string targetSceneName = "BasementScene";
        [SerializeField] private SceneFader sceneFader;

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
            // 이미 씬 전환 중이면 무시
            if (isTransitioning) return;

            // 잠겨있으면 메시지만 출력
            if (isLocked)
            {
                ShowMessage(lockedMessage);
                return;
            }

            // 잠금 해제됨 → 씬 이동
            StartSceneTransition();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 외부(UnifiedKeypad)에서 잠금 해제 시 호출
        /// </summary>
        public void Unlock()
        {
            isLocked = false;
            ShowMessage("문의 잠금이 해제되었다.");
        }

        /// <summary>
        /// 외부에서 잠금 상태 설정
        /// </summary>
        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }

        /// <summary>
        /// 잠금 상태 확인
        /// </summary>
        public bool IsLocked()
        {
            return isLocked;
        }

        #endregion

        #region Private Methods

        private void StartSceneTransition()
        {
            isTransitioning = true;

            // 전환 메시지 출력
            ShowMessage(transitionMessage);

            // 씬 이동 방식에 따라 분기
            if (SceneLoadManager.Instance != null)
            {
                // SceneLoadManager 사용
                if (sceneFader != null)
                {
                    sceneFader.FadeTo(targetSceneName);
                }

                SceneLoadManager.Instance.RequestMoveToScene(targetSceneName);
            }
            else
            {
                // 일반 씬 전환
                if (sceneFader != null)
                {
                    sceneFader.FadeTo(targetSceneName);
                }
                else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
                }
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

            return isLocked ? "문 열기" : "문 닫기";
        }

        #endregion
    }
}