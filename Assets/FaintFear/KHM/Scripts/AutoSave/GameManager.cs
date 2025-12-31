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
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != loadToScene)
            {
                EnterMenuState();
                return;
            }

            newGameSpawnPoint = GameObject.Find("StartSpawnPoint")?.transform;

            if (newGameSpawnPoint == null)
            {
                Debug.LogError("StartSpawnPoint not found!");
                return;
            }

            EnterGameplayState();

            if (shouldLoadGame)
            {
                shouldLoadGame = false;
                LoadPlayerWithCharacterController();
            }
            else
            {
                SpawnPlayerAtStart();
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
            PlayerMove move = player.GetComponent<PlayerMove>();

            if (move != null)
                move.enabled = false;

            if (cc != null)
                cc.enabled = false;

            // ✅ y 위치 안전 보정
            Vector3 safePos = data.playerPosition;
            safePos.y += 0.5f;

            player.transform.SetPositionAndRotation(
                safePos,
                data.playerRotation
            );

            // ⭐ 핵심: 한 프레임 대기 후 CC 활성화
            StartCoroutine(EnableCCNextFrame(cc, move));

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
            shouldLoadGame = true;
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
        private IEnumerator EnablePlayerMoveNextFrame(PlayerMove move)
        {
            yield return null;

            if (move != null)
                move.enabled = true;
        }
        private void SpawnPlayerAtStart()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;

            CharacterController cc = player.GetComponent<CharacterController>();
            PlayerMove move = player.GetComponent<PlayerMove>();

            StartCoroutine(NewGameSpawnRoutine(player, cc, move));
        }
        private IEnumerator NewGameSpawnRoutine(GameObject player, CharacterController cc,
            PlayerMove move)
        {
            if (move != null)
                move.enabled = false;

            if (cc != null)
                cc.enabled = false;

            Vector3 pos = newGameSpawnPoint.position;
            pos.y += 0.5f;

            player.transform.SetPositionAndRotation(
                pos,
                newGameSpawnPoint.rotation
            );

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
