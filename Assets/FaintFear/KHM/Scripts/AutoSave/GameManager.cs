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
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            var data = SaveSystem.LoadPreview();
            TutorialCompleted = data != null && data.tutorialCompleted;

            Debug.Log($"[GameManager] Initialized - TutorialCompleted: {TutorialCompleted}");
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
            if (scene.name != loadToScene)
            {
                EnterMenuState();
                return;
            }

            var data = SaveSystem.LoadPreview();
            TutorialCompleted = data != null && data.tutorialCompleted;

            Debug.Log($"[GameManager] Scene loaded - Mode: {currentStartMode}, TutorialCompleted: {TutorialCompleted}");

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
            Debug.Log("[GameManager] Starting NEW GAME");

            if (PlayerStatus.Instance != null)
            {
                PlayerStatus.Instance.ResetStatus();
            }

            SpawnPlayerAtStart();
        }

        private void HandleLoadGame()
        {
            Debug.Log("[GameManager] Loading saved game");

            if (!SaveSystem.HasSave())
            {
                Debug.LogWarning("[GameManager] No save file found! Starting new game instead.");
                HandleNewGame();
                return;
            }

            // ⭐ 페이드 효과와 함께 로드
            StartCoroutine(LoadGameWithFade());
        }

        // ⭐ 페이드 효과를 포함한 로드 코루틴
        private IEnumerator LoadGameWithFade()
        {
            // HUDManager 찾기
            HUDManager hudManager = FindFirstObjectByType<HUDManager>();

            if (hudManager != null)
            {
                Debug.Log("[GameManager] Starting fade from black for load game");

                // 플레이어 조작 잠금
                GameObject player = GameObject.FindWithTag("Player");
                PlayerMove playerMove = null;

                if (player != null)
                {
                    playerMove = player.GetComponent<PlayerMove>();
                    if (playerMove != null)
                    {
                        playerMove.canMove = false;
                        playerMove.SetLookLock(true);
                    }
                }

                // 약간의 딜레이
                yield return new WaitForSeconds(0.3f);

                // 데이터 로드 및 플레이어 배치
                LoadPlayerWithCharacterController();

                // 페이드 인 (검정 → 밝게)
                hudManager.FadeFromBlack();

                yield return new WaitForSeconds(1.5f);

                // 플레이어 조작 복구
                if (playerMove != null)
                {
                    playerMove.canMove = true;
                    playerMove.SetLookLock(false);
                }

                Debug.Log("[GameManager] Load game fade completed");
            }
            else
            {
                // HUDManager가 없으면 바로 로드
                Debug.LogWarning("[GameManager] HUDManager not found, loading without fade");
                LoadPlayerWithCharacterController();
            }
        }

        private void LoadPlayerWithCharacterController()
        {
            SaveData data = SaveSystem.LoadPreview();
            if (data == null)
            {
                Debug.LogError("[GameManager] Failed to load save data!");
                return;
            }

            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogError("[GameManager] Player not found!");
                return;
            }

            CharacterController cc = player.GetComponent<CharacterController>();
            PlayerMove move = player.GetComponent<PlayerMove>();

            if (move != null)
                move.enabled = false;

            if (cc != null)
                cc.enabled = false;

            Vector3 safePos = data.playerPosition;
            safePos.y += 0.5f;

            player.transform.SetPositionAndRotation(safePos, data.playerRotation);

            PlayerStatus.Instance.SetHealth(data.mental);
            PlayerStatus.Instance.currentBattery = data.battery;

            StartCoroutine(EnableCCNextFrame(cc, move));

            Debug.Log($"[GameManager] Loaded - Position: {data.playerPosition}, Mental: {data.mental}, Battery: {data.battery}");
        }

        public void StartNewGame()
        {
            Debug.Log("[GameManager] StartNewGame called");

            SaveSystem.DeleteSave();

            if (SaveSystem.HasSave())
            {
                Debug.LogError("[GameManager] Failed to delete save file!");
            }

            TutorialCompleted = false;

            if (PlayerStatus.Instance != null)
            {
                PlayerStatus.Instance.ResetStatus();
            }

            currentStartMode = GameStartMode.NewGame;

            SceneManager.LoadScene(loadToScene);
        }

        public void ContinueGame()
        {
            Debug.Log("[GameManager] ContinueGame called");

            if (!SaveSystem.HasSave())
            {
                Debug.LogWarning("[GameManager] No save file to continue!");
                return;
            }

            currentStartMode = GameStartMode.Continue;
            SceneManager.LoadScene(loadToScene);
        }

        public void RestartFromCheckpoint()
        {
            Debug.Log("[GameManager] RestartFromCheckpoint called");

            if (!SaveSystem.HasSave())
            {
                Debug.LogWarning("[GameManager] No checkpoint to restart from!");
                StartNewGame();
                return;
            }

            currentStartMode = GameStartMode.RestartFromCheckpoint;
            SceneManager.LoadScene(loadToScene);
        }

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

        private void SpawnPlayerAtStart()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogError("[GameManager] Player not found for spawn!");
                return;
            }

            CharacterController cc = player.GetComponent<CharacterController>();
            PlayerMove move = player.GetComponent<PlayerMove>();

            StartCoroutine(NewGameSpawnRoutine(player, cc, move));
        }

        private IEnumerator NewGameSpawnRoutine(GameObject player, CharacterController cc, PlayerMove move)
        {
            if (move != null)
                move.enabled = false;

            if (cc != null)
                cc.enabled = false;

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

            if (move != null)
                move.enabled = true;

            Debug.Log($"[GameManager] Player spawned at new game position: {pos}");
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

            if (move != null)
                move.enabled = true;
        }
    }
}