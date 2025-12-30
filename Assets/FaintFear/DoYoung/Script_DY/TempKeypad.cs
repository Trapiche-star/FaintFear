using UnityEngine;
using UnityEngine.SceneManagement;

namespace FaintFear
{
    /// <summary>
    /// 임시 키패드 상호작용 오브젝트
    /// </summary>
    public class TempKeypad : Interactive, IActionProvider
    {
        #region Variables

        [SerializeField] private string targetSceneName;          // 이동할 씬 이름
        [SerializeField] private SequenceTextManager sequenceText; // 시퀀스 메시지 출력

        private bool isOpened = false; // 지하실이 열렸는지 여부

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            if (!isOpened)
            {
                OpenBasement();
                return; // 1단계 처리 후 종료한다
            }

            MoveToScene();
            // 2단계에서 씬 이동을 실행한다
        }

        // 지하실이 열렸음을 처리한다
        private void OpenBasement()
        {
            isOpened = true;
            // 지하실이 열린 상태로 전환한다

            if (sequenceText != null)
                sequenceText.ShowMessage("지하실이 열렸다.");
            // 시퀀스 메시지를 출력한다
        }

        // 지정된 씬으로 이동한다
        private void MoveToScene()
        {
            if (string.IsNullOrEmpty(targetSceneName))
                return; // 씬 이름이 없으면 이동하지 않는다

            SceneManager.LoadScene(targetSceneName);
            // 지정된 씬으로 전환한다
        }

        #endregion


        #region Property

        // Action UI에 표시할 문구를 제공한다
        public string GetActionText()
        {
            return isOpened ? "이동하기" : "사용하기";
            // 상태에 따라 액션 문구를 변경한다
        }

        #endregion
    }
}
