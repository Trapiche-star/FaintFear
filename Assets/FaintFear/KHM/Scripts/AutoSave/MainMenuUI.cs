using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 메인메뉴 UI를 관리하는 클래스
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        public GameObject continueButton;
        public GameObject mainMenuPanel;
        public GameObject optionsPanel;

        void Start()
        {
            continueButton.SetActive(SaveSystem.HasSave());
        }

        //새 게임 
        public void OnNewGame()
        {
            //경고 팝업 띄워도 됨
            GameManager.Instance.StartNewGame();
        }

        //이어하기
        public void OnContinue()
        {
            GameManager.Instance.ContinueGame();
        }

        //옵션
        public void OnOptions()
        {
            mainMenuPanel.SetActive(false);
            optionsPanel.SetActive(true);
        }

        //게임 종료
        public void OnQuit()
        {
            Debug.Log("Quit 버튼을 눌렀습니다 - 모든 데이터 초기화");

            // PlayerPrefs 삭제
            PlayerPrefs.DeleteAll();

            // ⭐ SaveSystem의 JSON 파일도 삭제
            SaveSystem.DeleteSave();

            // ⭐ GameManager의 튜토리얼 상태도 초기화
            if (GameManager.Instance != null)
            {
                GameManager.TutorialCompleted = false;
            }

            Application.Quit();
        }
    }
}