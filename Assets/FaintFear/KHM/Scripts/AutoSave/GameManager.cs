using UnityEngine;
using UnityEngine.SceneManagement;

namespace FaintFear
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [SerializeField] private string loadToScene = "Level01";

        public bool shouldLoadGame;

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

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == loadToScene) // Level01
            {
                EnterGameplayState();

                if (shouldLoadGame)
                {
                    shouldLoadGame = false;
                    LoadPlayerWithCharacterController();
                }
            }
            else
            {
                EnterMenuState();
            }
        }
        public static void EnterMenuState()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 1f;
        }

        private void LoadPlayerWithCharacterController()
        {
            if (!SaveSystem.HasSave()) return;

            SaveData data = SaveSystem.LoadPreview();
            if (data == null) return;

            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;

            CharacterController cc = player.GetComponent<CharacterController>();

            if (cc != null)
            {
                cc.enabled = false;

                player.transform.SetPositionAndRotation(
                    data.playerPosition,
                    data.playerRotation
                );

                cc.enabled = true;
                cc.Move(Vector3.zero); // ⭐ 핵심
            }
            else
            {
                player.transform.SetPositionAndRotation(
                    data.playerPosition,
                    data.playerRotation
                );
            }

            // 상태 복원
            PlayerStatus.Instance.SetHealth(data.mental);
            PlayerStatus.Instance.currentBattery = data.battery;
        }


        //새게임 
        public void StartNewGame()
        {
            SaveSystem.DeleteSave();
            shouldLoadGame = false;
            SceneManager.LoadScene(loadToScene);
        }

        //이어하기
        public void ContinueGame()
        {
            shouldLoadGame = true;
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

        public static void EnterGameplayState()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;
        }
    }
}
