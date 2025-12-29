using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 메인메뉴 UI를 관리하는 클래스
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        public GameObject continueButton;

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

        //게임 종료
        public void OnQuit()
        {
            Application.Quit();
        }
    }
}