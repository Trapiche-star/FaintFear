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

        // ⭐ 제거: 사용하지 않는 변수
        // private static string nextSpawnPointName;

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

                // ⭐ 추가: 시작 시 페이드 인
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
            // 1. 페이드 아웃
            yield return FadeOut();

            // 2. 씬 로드
            currentSceneName = sceneName;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            // 로딩 진행률 표시 (선택사항)
            while (!asyncLoad.isDone)
            {
                // 필요하면 로딩 바 업데이트
                yield return null;
            }

            // 3. 씬 로드 완료 후 대기
            yield return new WaitForEndOfFrame();

            // 4. 페이드 인
            yield return FadeIn();

            IsSceneTransitioning = false;

            Debug.Log($"[SceneLoadManager] 씬 로드 완료: {sceneName}");
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

        // ⭐ 추가: 디버그용 - 현재 상태 확인
        public string GetCurrentStatus()
        {
            return $"현재 씬: {currentSceneName}, 전환 중: {IsSceneTransitioning}";
        }
    }
}