using UnityEngine;

namespace FaintFear
{
    public class MainMenuUI : MonoBehaviour
    {
        public GameObject continueButton;
        public GameObject mainMenuPanel;
        public GameObject optionsPanel;

        private void Start()
        {
            if (continueButton != null)
                continueButton.SetActive(SaveSystem.HasSave());
        }

        // 새 게임
        public void OnNewGame()
        {
            // 🔴 에디터에서 눌렀을 경우 차단
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[MainMenuUI] Play 모드에서만 실행됩니다.");
                return;
            }

            if (GameManager.Instance == null)
            {
                Debug.LogError("[MainMenuUI] GameManager가 씬에 없습니다!");
                return;
            }

            GameManager.Instance.StartNewGame();
        }

        // 이어하기
        public void OnContinue()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[MainMenuUI] Play 모드에서만 실행됩니다.");
                return;
            }

            if (GameManager.Instance == null)
            {
                Debug.LogError("[MainMenuUI] GameManager가 씬에 없습니다!");
                return;
            }

            GameManager.Instance.ContinueGame();
        }

        // 옵션
        public void OnOptions()
        {
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);

            if (optionsPanel != null)
                optionsPanel.SetActive(true);
        }

        // 종료
        public void OnQuit()
        {
            if (!Application.isPlaying)
                return;

            PlayerPrefs.DeleteAll();
            SaveSystem.DeleteSave();
            GameManager.TutorialCompleted = false;

            Application.Quit();
        }
    }
}
