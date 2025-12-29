using UnityEngine;
using UnityEngine.SceneManagement;

namespace FaintFear
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [SerializeField] private string loadToScene = "Level01";

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        //새게임 
        public void StartNewGame()
        {
            SaveSystem.DeleteSave();
            SceneManager.LoadScene(loadToScene);
        }

        //이어하기
        public void ContinueGame()
        {
            SceneManager.LoadScene(loadToScene);
        }

        //게임 오버
        public void RestartFromCheckpoint()
        {
            SceneManager.LoadScene(loadToScene);
        }

        //메인메뉴로 가기
        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
