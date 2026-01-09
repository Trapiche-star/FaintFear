using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                // ⭐ 튜토리얼 상태도 함께 저장
                SaveSystem.SaveGame("cheatKey_Save", TutorialCompleted, true);
                Debug.Log("[GameManager] 치트키 자동 저장 완료 (모든 상태 병합)");
            }
        }

        // =========================
        // Menu
        // =========================

        public void StartNewGame()
        {
            // ⭐ 1. 세이브 파일 삭제
            SaveSystem.DeleteSave();

            // ⭐ 2. 튜토리얼 상태 초기화
            TutorialCompleted = false;

            // ⭐ 3. 모든 DontDestroyOnLoad 매니저 초기화
            ResetAllManagers();

            // ⭐ 4. 런타임 상태 완전 초기화
            RuntimeStateManager.ClearRuntimeState();

            //DestroyGlobalSingletons();

            // ⭐ 5. 게임 시작 모드 설정
            currentStartMode = GameStartMode.NewGame;

            Debug.Log("[GameManager] NewGame - 모든 데이터 초기화 완료");

            // ⭐ 6. Intro 씬 로드
            SceneManager.LoadScene("Intro");
        }

        public void ContinueGame()
        {
            currentStartMode = GameStartMode.Continue;

            SaveData data = SaveSystem.LoadPreview();
            string sceneToLoad = data != null ? data.savedSceneName : "Level01";

            Debug.Log($"[GameManager] ContinueGame - 씬 로드: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }

        public void RestartFromCheckpoint()
        {
            currentStartMode = GameStartMode.RestartFromCheckpoint;

            SaveData data = SaveSystem.LoadPreview();
            string sceneToLoad = data != null ? data.savedSceneName : "Level01";

            Debug.Log($"[GameManager] RestartFromCheckpoint - 씬 로드: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }

        // ⭐ 추가: 모든 DontDestroyOnLoad 매니저 초기화
        private void ResetAllManagers()
        {
            // PlayerStatus 초기화
            if (PlayerStatus.Instance != null)
            {
                PlayerStatus.Instance.ResetStatus();
                Debug.Log("[GameManager] PlayerStatus 초기화");
            }

            // EndingManager 초기화
            if (EndingManager.Instance != null)
            {
                EndingManager.Instance.ResetState();
                Debug.Log("[GameManager] EndingManager 초기화");
            }

            // DocumentPuzzleManager 초기화
            if (DocumentPuzzleManager.Instance != null)
            {
                DocumentPuzzleManager.Instance.ResetState();
                Debug.Log("[GameManager] DocumentPuzzleManager 초기화");
            }

            // ⭐ 추가: ElevatorManager 초기화
            if (ElevatorManager.Instance != null)
            {
                ElevatorManager.Instance.ResetState();
                Debug.Log("[GameManager] ElevatorManager 초기화");
            }

            if (PuzzleInventory.Instance != null)
            {
                PuzzleInventory.Instance.ResetInventory();
                Debug.Log("[GameManager] PuzzleInventory 초기화");
            }
            // SceneLoadManager는 상태가 없으므로 초기화 불필요
        }

        // =========================
        // Scene Load
        // =========================

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[GameManager] 씬 로드됨: {scene.name}, 모드: {currentStartMode}");

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
                    Debug.Log("[GameManager] NewGame 모드 - 플레이어 스폰 및 초기화");
                    RuntimeStateManager.ClearRuntimeState();
                    SpawnPlayerAtSpawnPoint(newGameSpawnPointName);
                    PlayerStatus.Instance?.ResetStatus();
                    break;

                case GameStartMode.Continue:
                case GameStartMode.RestartFromCheckpoint:
                    {
                        Debug.Log($"[GameManager] {modeToExecute} 모드 - 저장 데이터 로드");

                        SaveData data = SaveSystem.LoadPreview();

                        // ⭐ 1. SaveData → RuntimeState 복원
                        RuntimeStateManager.RestoreRuntimeStateFromSaveData(data);

                        // ⭐ 2. 플레이어 로드
                        LoadPlayerFromSave();

                        // ⭐ 3. 1프레임 대기 후 Runtime 적용
                        StartCoroutine(ApplyRuntimeStateDelayed());
                        break;
                    }

                case GameStartMode.SceneTransition:
                    Debug.Log("[GameManager] SceneTransition 모드 - 런타임 상태 적용");
                    SpawnPlayerAtSpawnPoint(sceneTransitionSpawnPoint);
                    RuntimeStateManager.ApplyRuntimeState();
                    break;

                default:
                    Debug.LogWarning("[GameManager] 알 수 없는 게임 모드");
                    break;
            }

            sceneTransitionSpawnPoint = "";
        }
        private IEnumerator ApplyWorldObjectLoadDelayed()
        {
            // 1프레임 대기 (모든 Awake/Start 완료 대기)
            yield return new WaitForEndOfFrame();

            Debug.Log("[GameManager] 월드 오브젝트 로드 시작 (딜레이 후)");
            SaveSystem.ApplyWorldObjectLoad();

            Debug.Log("[GameManager] 월드 오브젝트 로드 완료");
        }
        private IEnumerator ApplyRuntimeStateDelayed()
        {
            yield return new WaitForEndOfFrame();
            RuntimeStateManager.ApplyRuntimeState();
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
                Debug.LogWarning("[GameManager] 저장 데이터 없음 - 기본 스폰 위치 사용");
                SpawnPlayerAtSpawnPoint(newGameSpawnPointName);
                PlayerStatus.Instance?.ResetStatus();
                return;
            }

            SpawnPlayer(data.playerPosition, data.playerRotation);

            // 플레이어 상태 복원
            PlayerStatus.Instance?.SetHealth(data.mental);
            PlayerStatus.Instance.currentBattery = data.battery;
            PlayerStatus.Instance.batteryCount = data.batteryCount;
            PlayerStatus.Instance.Load(data);
            Debug.Log($"[GameManager] 플레이어 상태 로드 완료 - " +
                     $"체력:{data.mental}, 배터리:{data.battery}, 배터리 개수:{data.batteryCount}");
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

        // *추가 현재 게임이 NewGame 모드인지 여부
        public bool IsNewGame => currentStartMode == GameStartMode.NewGame;

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