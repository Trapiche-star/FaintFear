using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace FaintFear
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Player")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private string newGameSpawnPointName = "NewGameSpawn";

        [Header("Gameplay Scenes")]
        [SerializeField] private List<string> gameplayScenes;

        public static bool TutorialCompleted;

        private static string sceneTransitionSpawnPoint = "";
        private static GameStartMode pendingStartMode = GameStartMode.None;

        private enum GameStartMode
        {
            None,
            NewGame,
            Continue,
            RestartFromCheckpoint,
            SceneTransition
        }

        private GameStartMode currentStartMode = GameStartMode.None;

        public void SetSceneTransitionMode(string spawnPointName)
        {
            pendingStartMode = GameStartMode.SceneTransition;
            sceneTransitionSpawnPoint = spawnPointName;
            Debug.Log($"[GameManager] SceneTransition 예약: {spawnPointName}");
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
        // Menu
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

            SaveData data = SaveSystem.LoadPreview();
            string sceneToLoad = data != null ? data.savedSceneName : "Level01";
            SceneManager.LoadScene(sceneToLoad);
        }

        public void RestartFromCheckpoint()
        {
            currentStartMode = GameStartMode.RestartFromCheckpoint;

            SaveData data = SaveSystem.LoadPreview();
            string sceneToLoad = data != null ? data.savedSceneName : "Level01";
            SceneManager.LoadScene(sceneToLoad);
        }

        // =========================
        // Scene Load
        // =========================

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!gameplayScenes.Contains(scene.name))
            {
                EnterMenuState();
                return;
            }

            EnterGameplayState();

            GameStartMode modeToExecute =
                pendingStartMode != GameStartMode.None
                ? pendingStartMode
                : currentStartMode;

            pendingStartMode = GameStartMode.None;
            currentStartMode = GameStartMode.None;

            switch (modeToExecute)
            {
                case GameStartMode.NewGame:
                    RuntimeStateManager.ClearRuntimeState();
                    SpawnPlayerAtSpawnPoint(newGameSpawnPointName);
                    PlayerStatus.Instance?.ResetStatus();
                    break;

                case GameStartMode.Continue:
                case GameStartMode.RestartFromCheckpoint:
                    RuntimeStateManager.ClearRuntimeState();
                    LoadPlayerFromSave();
                    SaveSystem.ApplyWorldObjectLoad();
                    break;

                case GameStartMode.SceneTransition:
                    SpawnPlayerAtSpawnPoint(sceneTransitionSpawnPoint);
                    RuntimeStateManager.ApplyRuntimeState();
                    break;
            }

            sceneTransitionSpawnPoint = "";
        }

        // =========================
        // Player
        // =========================

        public void SpawnPlayerAtSpawnPoint(string spawnPointName)
        {
            GameObject spawn = GameObject.Find(spawnPointName);
            if (spawn == null)
            {
                Debug.LogError($"[GameManager] SpawnPoint 없음: {spawnPointName}");
                return;
            }

            SpawnPlayer(spawn.transform.position, spawn.transform.rotation);
        }

        private void SpawnPlayer(Vector3 pos, Quaternion rot)
        {
            GameObject old = GameObject.FindWithTag("Player");
            if (old) Destroy(old);

            GameObject player = Instantiate(playerPrefab, pos, rot);
            BindPlayerSystems(player);
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
            // UI
            var ui = FindFirstObjectByType<FlashlightUI>();
            ui?.BindPlayer(player);

            // 튜토리얼 이벤트
            foreach (var t in FindObjectsByType<TutorialEventBase>(FindObjectsSortMode.None))
                t.BindPlayer(player);

            // 트리거 제한
            foreach (var r in FindObjectsByType<TriggerRestrict>(FindObjectsSortMode.None))
                r.BindPlayer(player);

            // 손전등 입력
            var flashInteraction = player.GetComponent<PlayerFlashLightInteraction>();
            flashInteraction?.BindFlashlight(player);

            // 캐릭터 컨트롤러 리셋
            var cc = player.GetComponent<CharacterController>();
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