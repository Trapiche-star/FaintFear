using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace FaintFear
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private string loadToScene = "Level01";
        private Transform newGameSpawnPoint;
        public static bool TutorialCompleted;

        private enum GameStartMode
        {
            NewGame,
            Continue,
            RestartFromCheckpoint
        }

        private GameStartMode currentStartMode = GameStartMode.NewGame;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            var data = SaveSystem.LoadPreview();
            TutorialCompleted = data != null && data.tutorialCompleted;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // =========================
        // 🔹 메인메뉴에서 호출되는 함수들
        // =========================

        public void StartNewGame()
        {
            SaveSystem.DeleteSave();
            TutorialCompleted = false;
            currentStartMode = GameStartMode.NewGame;

            SceneManager.LoadScene(loadToScene);
        }

        public void ContinueGame()
        {
            if (!SaveSystem.HasSave())
            {
                Debug.LogWarning("[GameManager] No save file to continue");
                StartNewGame();
                return;
            }

            currentStartMode = GameStartMode.Continue;
            SceneManager.LoadScene(loadToScene);
        }

        public void RestartFromCheckpoint()
        {
            currentStartMode = GameStartMode.RestartFromCheckpoint;
            SceneManager.LoadScene(loadToScene);
        }

        // =========================

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != loadToScene)
            {
                EnterMenuState();
                return;
            }

            //상시 탐색 BGM
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayBGM("BGM_Explore");

            var data = SaveSystem.LoadPreview();
            TutorialCompleted = data != null && data.tutorialCompleted;

            newGameSpawnPoint = GameObject.Find("StartSpawnPoint")?.transform;
            if (newGameSpawnPoint == null)
            {
                Debug.LogError("StartSpawnPoint not found!");
                return;
            }

            EnterGameplayState();

            switch (currentStartMode)
            {
                case GameStartMode.NewGame:
                    HandleNewGame();
                    break;

                case GameStartMode.Continue:
                case GameStartMode.RestartFromCheckpoint:
                    HandleLoadGame();
                    break;
            }

            currentStartMode = GameStartMode.NewGame;
        }

        public static void EnterMenuState()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 1f;
        }

        private void HandleNewGame()
        {
            if (PlayerStatus.Instance != null)
                PlayerStatus.Instance.ResetStatus();

            SpawnPlayerAtStart();
        }

        private void HandleLoadGame()
        {
            if (!SaveSystem.HasSave())
            {
                HandleNewGame();
                return;
            }

            StartCoroutine(LoadGameWithFade());
        }

        private IEnumerator LoadGameWithFade()
        {
            HUDManager hudManager = FindFirstObjectByType<HUDManager>();

            GameObject player = GameObject.FindWithTag("Player");
            PlayerMove move = player?.GetComponent<PlayerMove>();

            if (move != null)
            {
                move.canMove = false;
                move.SetLookLock(true);
            }

            yield return new WaitForSeconds(0.3f);

            LoadPlayerWithCharacterController();

            if (hudManager != null)
                hudManager.FadeFromBlack();

            yield return new WaitForSeconds(1.5f);

            if (move != null)
            {
                move.canMove = true;
                move.SetLookLock(false);
            }
        }

        private void LoadPlayerWithCharacterController()
        {
            SaveData data = SaveSystem.LoadPreview();
            if (data == null) return;

            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;

            CharacterController cc = player.GetComponent<CharacterController>();
            PlayerMove move = player.GetComponent<PlayerMove>();

            if (move != null) move.enabled = false;
            if (cc != null) cc.enabled = false;

            Vector3 pos = data.playerPosition;
            pos.y += 0.5f;

            player.transform.SetPositionAndRotation(pos, data.playerRotation);

            PlayerStatus.Instance.SetHealth(data.mental);
            PlayerStatus.Instance.currentBattery = data.battery;

            StartCoroutine(EnableCCNextFrame(cc, move));
        }

        public void GoToMainMenu()
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.StopBGM();

            SceneManager.LoadScene("MainMenu");
        }

        public static void EnterGameplayState()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }

        private void SpawnPlayerAtStart()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;

            CharacterController cc = player.GetComponent<CharacterController>();
            PlayerMove move = player.GetComponent<PlayerMove>();

            StartCoroutine(NewGameSpawnRoutine(player, cc, move));
        }

        private IEnumerator NewGameSpawnRoutine(GameObject player, CharacterController cc, PlayerMove move)
        {
            if (move != null) move.enabled = false;
            if (cc != null) cc.enabled = false;

            Vector3 pos = newGameSpawnPoint.position;
            pos.y += 0.5f;

            player.transform.SetPositionAndRotation(pos, newGameSpawnPoint.rotation);

            yield return new WaitForEndOfFrame();

            if (cc != null)
            {
                cc.enabled = true;
                cc.Move(Vector3.zero);
            }

            yield return null;

            if (move != null) move.enabled = true;
        }

        private IEnumerator EnableCCNextFrame(CharacterController cc, PlayerMove move)
        {
            yield return new WaitForEndOfFrame();

            if (cc != null)
            {
                cc.enabled = true;
                cc.Move(Vector3.zero);
            }

            yield return null;

            if (move != null) move.enabled = true;
        }
    }
}
