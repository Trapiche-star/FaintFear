using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 씬 전환 및 페이드 전용 관리자
    /// </summary>
    public class SceneLoadManager : MonoBehaviour
    {
        public static SceneLoadManager Instance { get; private set; }

        [Header("Fade")]
        [SerializeField] private Image fadeImage;
        [SerializeField] private AnimationCurve fadeCurve;

        private string currentSceneName;

        public static bool IsSceneTransitioning { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            currentSceneName = SceneManager.GetActiveScene().name;
        }

        private void Start()
        {
            if (fadeImage != null)
            {
                fadeImage.color = Color.black;
                StartCoroutine(FadeIn());
            }
        }

        // =========================
        // Scene Load
        // =========================

        public void LoadScene(string sceneName, string spawnPointName = "")
        {
            if (IsSceneTransitioning)
            {
                Debug.LogWarning("[SceneLoadManager] 이미 씬 전환 중입니다.");
                return;
            }

            IsSceneTransitioning = true;

            // ⭐ GameManager에 씬 전환 모드 설정
            if (!string.IsNullOrEmpty(spawnPointName) && GameManager.Instance != null)
            {
                GameManager.Instance.SetSceneTransitionMode(spawnPointName);
            }

            Debug.Log($"[SceneLoadManager] 씬 로드 시작: {sceneName}, 스폰: {spawnPointName}");
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            // ⭐ 1. 씬 이동 전에 현재 상태 저장
            SaveBeforeSceneTransition();

            // 2. 페이드 아웃
            yield return FadeOut();

            // 3. 씬 로드
            currentSceneName = sceneName;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // 4. 씬 로드 완료 후 대기
            yield return new WaitForEndOfFrame();

            // 5. 페이드 인
            yield return FadeIn();

            IsSceneTransitioning = false;

            Debug.Log($"[SceneLoadManager] 씬 로드 완료: {sceneName}");
        }

        // ⭐ 추가: 씬 전환 전 저장
        private void SaveBeforeSceneTransition()
        {
            // 게임플레이 씬에서만 저장 (메뉴 등은 제외)
            string currentScene = SceneManager.GetActiveScene().name;

            // 메뉴나 인트로 씬은 저장하지 않음
            if (currentScene == "MainMenu" || currentScene == "Intro")
            {
                Debug.Log("[SceneLoadManager] 메뉴/인트로 씬은 저장 생략");
                return;
            }

            // 플레이어가 존재하는지 확인 (게임플레이 중인지)
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.Log("[SceneLoadManager] 플레이어 없음 - 저장 생략");
                return;
            }

            // ⭐ 핵심: 런타임 상태를 SaveData에 병합한 후 저장
            Debug.Log("[SceneLoadManager] 씬 전환 전 자동 저장 실행 (런타임 상태 포함)");

            // 기존 저장 파일 로드
            SaveData data = SaveSystem.LoadPreview() ?? new SaveData();

            // 현재 플레이어 상태 업데이트
            data.mental = PlayerStatus.Instance.currentMentalPower;
            data.battery = PlayerStatus.Instance.currentBattery;
            data.batteryCount = PlayerStatus.Instance.batteryCount;
            data.playerPosition = player.transform.position;
            data.playerRotation = player.transform.rotation;
            data.savedSceneName = currentScene;
            data.tutorialCompleted = GameManager.TutorialCompleted;

            // ⭐ 런타임 상태를 SaveData에 병합
            RuntimeStateManager.MergeRuntimeStateToSaveData(ref data);

            // 저장
            System.IO.File.WriteAllText(
                System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "save.json"),
                JsonUtility.ToJson(data, true)
            );

            Debug.Log("[SceneLoadManager] 씬 전환 자동 저장 완료 (런타임 상태 병합됨)");
        }

        // =========================
        // Fade
        // =========================

        private IEnumerator FadeIn()
        {
            if (fadeImage == null)
            {
                Debug.LogWarning("[SceneLoadManager] fadeImage가 없습니다.");
                yield break;
            }

            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                float a = fadeCurve != null ? fadeCurve.Evaluate(t) : t;
                fadeImage.color = new Color(0, 0, 0, a);
                yield return null;
            }

            fadeImage.color = Color.clear;
        }

        private IEnumerator FadeOut()
        {
            if (fadeImage == null)
            {
                Debug.LogWarning("[SceneLoadManager] fadeImage가 없습니다.");
                yield break;
            }

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                float a = fadeCurve != null ? fadeCurve.Evaluate(t) : t;
                fadeImage.color = new Color(0, 0, 0, a);
                yield return null;
            }

            fadeImage.color = Color.black;
        }

        public string GetCurrentStatus()
        {
            return $"현재 씬: {currentSceneName}, 전환 중: {IsSceneTransitioning}";
        }
    }
}