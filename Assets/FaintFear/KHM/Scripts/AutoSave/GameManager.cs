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

        private void Update()
        {
            // ⭐ 치트키: G키로 즉시 저장
            if (Input.GetKeyDown(KeyCode.G))
            {
                CheatSave();
            }
        }
        private void CheatSave()
        {
            // 현재 게임플레이 씬에 있을 때만 작동
            if (!gameplayScenes.Contains(SceneManager.GetActiveScene().name))
            {
                Debug.Log("[Cheat] 게임플레이 씬에서만 저장 가능합니다");
                return;
            }

            SaveSystem.SaveGame(
                checkpointId: "cheat_save",
                tutorialCompleted: TutorialCompleted,
                saveWorldObjects: true
            );

            Debug.Log("=== [CHEAT] G키 저장 완료! ===");
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
            Debug.Log($"[GameManager] OnSceneLoaded: {scene.name}, pendingMode: {pendingStartMode}, currentMode: {currentStartMode}");

            if (!gameplayScenes.Contains(scene.name))
            {
                EnterMenuState();
                return;
            }

            EnterGameplayState();

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayBGM("BGM_Explore");

            GameStartMode modeToExecute = pendingStartMode != GameStartMode.NewGame ? pendingStartMode : currentStartMode;

            Debug.Log($"[GameManager] Executing mode: {modeToExecute}");

            pendingStartMode = GameStartMode.NewGame;
            currentStartMode = GameStartMode.NewGame;
            string spawnToUse = sceneTransitionSpawnPoint;
            sceneTransitionSpawnPoint = "";

            switch (modeToExecute)
            {
                case GameStartMode.NewGame:
                    Debug.Log($"[GameManager] Spawning at NewGame spawn: {newGameSpawnPointName}");
                    RuntimeStateManager.ClearRuntimeState(); // ⭐ 새 게임 시 런타임 상태 초기화
                    SpawnPlayerAtSpawnPoint(newGameSpawnPointName);
                    PlayerStatus.Instance?.ResetStatus();
                    break;

                case GameStartMode.Continue:
                case GameStartMode.RestartFromCheckpoint:
                    Debug.Log("[GameManager] Loading player from save");
                    RuntimeStateManager.ClearRuntimeState(); // ⭐ 이어하기 시 런타임 상태 초기화
                    LoadPlayerFromSave();
                    SaveSystem.ApplyWorldObjectLoad();
                    break;

                case GameStartMode.SceneTransition:
                    Debug.Log($"[GameManager] SceneTransition mode - spawning at: {spawnToUse}");
                    SpawnPlayerAtSpawnPoint(spawnToUse);
                    SaveSystem.ApplyWorldObjectLoad(); // 저장된 상태 적용
                    RuntimeStateManager.ApplyRuntimeState(); // ⭐ 런타임 상태 적용
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

            var restricts = FindObjectsByType<TriggerRestrict>(FindObjectsSortMode.None);
            foreach (var r in restricts)
                r.BindPlayer(player);

            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                cc.enabled = true;
                cc.Move(Vector3.zero);
            }

            var flashInteraction = player.GetComponent<PlayerFlashLightInteraction>();
            flashInteraction?.BindFlashlight(player);
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