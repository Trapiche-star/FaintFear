using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace FaintFear
{
    /// <summary>
    /// 게임 전체 상태 + 플레이어 생명주기 전담 관리자
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Player")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private string newGameSpawnPointName = "NewGameSpawn";

        [Header("Gameplay Scenes")]
        [SerializeField] private List<string> gameplayScenes;

        public static bool TutorialCompleted;

        // ⭐ static으로 변경 - 씬 로드 시에도 유지됨!
        private static string sceneTransitionSpawnPoint = "";
        private static GameStartMode pendingStartMode = GameStartMode.NewGame;

        private enum GameStartMode
        {
            NewGame,
            Continue,
            RestartFromCheckpoint,
            SceneTransition
        }

        private GameStartMode currentStartMode = GameStartMode.NewGame;

        public void SetSceneTransitionMode(string spawnPointName)
        {
            // ⭐ static 변수에 저장
            pendingStartMode = GameStartMode.SceneTransition;
            sceneTransitionSpawnPoint = spawnPointName;
            Debug.Log($"[GameManager] SetSceneTransitionMode: {spawnPointName}, pendingMode: {pendingStartMode}");
        }

        private void Awake()
        {
            if (Instance != null)
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
        // Main Menu
        // =========================

        public void StartNewGame()
        {
            SaveSystem.DeleteSave();
            TutorialCompleted = false;
            currentStartMode = GameStartMode.NewGame;

            SceneManager.LoadScene("Intro");
        }

        public void ContinueGame()
        {
            currentStartMode = GameStartMode.Continue;
            SceneManager.LoadScene("Level01");
        }

        public void RestartFromCheckpoint()
        {
            currentStartMode = GameStartMode.RestartFromCheckpoint;
            SceneManager.LoadScene("Level01");
        }

        // =========================

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // ⭐ pendingStartMode 확인
            Debug.Log($"[GameManager] OnSceneLoaded: {scene.name}, pendingMode: {pendingStartMode}, currentMode: {currentStartMode}");

            if (!gameplayScenes.Contains(scene.name))
            {
                EnterMenuState();
                return;
            }

            EnterGameplayState();

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayBGM("BGM_Explore");

            // ⭐ pendingStartMode가 있으면 그걸 사용
            GameStartMode modeToExecute = pendingStartMode != GameStartMode.NewGame ? pendingStartMode : currentStartMode;

            Debug.Log($"[GameManager] Executing mode: {modeToExecute}");

            // ⭐ 실행 후 초기화
            pendingStartMode = GameStartMode.NewGame;
            currentStartMode = GameStartMode.NewGame;
            string spawnToUse = sceneTransitionSpawnPoint;
            sceneTransitionSpawnPoint = "";

            // ⭐ 저장된 모드로 실행
            switch (modeToExecute)
            {
                case GameStartMode.NewGame:
                    Debug.Log($"[GameManager] Spawning at NewGame spawn: {newGameSpawnPointName}");
                    SpawnPlayerAtSpawnPoint(newGameSpawnPointName);
                    PlayerStatus.Instance?.ResetStatus();
                    break;

                case GameStartMode.Continue:
                case GameStartMode.RestartFromCheckpoint:
                    Debug.Log("[GameManager] Loading player from save");
                    LoadPlayerFromSave();
                    break;

                case GameStartMode.SceneTransition:
                    Debug.Log($"[GameManager] SceneTransition mode - spawning at: {spawnToUse}");
                    SpawnPlayerAtSpawnPoint(spawnToUse);
                    break;
            }
        }

        // =========================
        // Player
        // =========================

        public void SpawnPlayerAtSpawnPoint(string spawnPointName)
        {
            Debug.Log($"[GameManager] SpawnPlayerAtSpawnPoint: {spawnPointName}");

            GameObject spawnPoint = GameObject.Find(spawnPointName);
            if (spawnPoint == null)
            {
                Debug.LogError($"[GameManager] SpawnPoint '{spawnPointName}' not found!");
                return;
            }

            Debug.Log($"[GameManager] SpawnPoint found at: {spawnPoint.transform.position}");

            SpawnPlayer(spawnPoint.transform.position, spawnPoint.transform.rotation);
        }

        private void SpawnPlayer(Vector3 position, Quaternion rotation)
        {
            GameObject oldPlayer = GameObject.FindWithTag("Player");
            if (oldPlayer != null)
            {
                Debug.Log("[GameManager] Destroying old player");
                Destroy(oldPlayer);
            }

            Debug.Log($"[GameManager] Instantiating player at: {position}");
            GameObject player = Instantiate(playerPrefab, position, rotation);

            BindPlayerSystems(player);

            Debug.Log($"[GameManager] Player spawned at: {player.transform.position}");
        }

        private void LoadPlayerFromSave()
        {
            SaveData data = SaveSystem.LoadPreview();

            if (data == null)
            {
                SpawnPlayerAtSpawnPoint(newGameSpawnPointName);
                return;
            }

            SpawnPlayer(data.playerPosition, data.playerRotation);

            PlayerStatus.Instance?.SetHealth(data.mental);
            PlayerStatus.Instance.currentBattery = data.battery;
            PlayerStatus.Instance.batteryCount = data.batteryCount;
        }

        private void BindPlayerSystems(GameObject player)
        {
            FlashlightUI ui = FindFirstObjectByType<FlashlightUI>();
            ui?.BindPlayer(player);

            var tutorials = FindObjectsByType<TutorialEventBase>(FindObjectsSortMode.None);
            foreach (var t in tutorials)
                t.BindPlayer(player);

            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                cc.enabled = true;
                cc.Move(Vector3.zero);
            }
        }

        // =========================
        // State
        // =========================

        public static void EnterMenuState()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 1f;
        }

        public static void EnterGameplayState()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }

        public void GoToMainMenu()
        {
            SoundManager.Instance?.StopBGM();
            SceneManager.LoadScene("MainMenu");
        }
    }
}