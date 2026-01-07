using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 씬 이동 전용 문
    /// 잠금 해제는 LockedDoorBase를 따르되, 열림 동작은 씬 전환으로 대체
    /// </summary>
    public class SceneTransitionDoor : LockedDoorBase, IActionProvider
    {
        #region Variables

        [Header("Scene Settings")]
        [SerializeField] private string targetSceneName = "BasementScene";
        [SerializeField] private string spawnPointName = "FromBasement";

        [Header("Messages")]
        [SerializeField, TextArea]
        private string transitionMessage = "문을 열고 들어간다...";

        private bool isTransitioning = false;

        #endregion

        #region LockedDoorBase Overrides

        // 키패드로만 여는 문 → 직접 해제 조건 없음
        protected override bool CanUnlock()
        {
            return false;
        }

        // 문을 여는 대신 씬 이동
        protected override void ToggleDoor()
        {
            if (isTransitioning) return;

            isTransitioning = true;

            if (sequenceText != null)
                sequenceText.ShowMessage(transitionMessage);

            if (SceneLoadManager.Instance != null)
            {
                SceneLoadManager.Instance.LoadScene(targetSceneName, spawnPointName);
            }
            else
            {
                Debug.LogError("[SceneTransitionDoor] SceneLoadManager not found!");
            }
        }

        // 물리적 회전 없음
        protected override void ApplyDoorRotation()
        {
            // 씬 이동 문은 회전 상태 없음
        }

        #endregion

        #region IActionProvider

        public string GetActionText()
        {
            if (isTransitioning)
                return string.Empty;

            return "[E] 문 열기";
        }

        #endregion
    }
}
