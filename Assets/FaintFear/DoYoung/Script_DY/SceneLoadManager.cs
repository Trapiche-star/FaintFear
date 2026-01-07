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
        private static string nextSpawnPointName;
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
                fadeImage.color = Color.black;
        }

        // =========================
        // Scene Load
        // =========================

        public void LoadScene(string sceneName, string spawnPointName = "")
        {
            if (IsSceneTransitioning)
                return;

            IsSceneTransitioning = true;

            // ⭐ 씬 이동 직전 저장
            //SaveSystem.SaveGame(checkpointId: "", tutorialCompleted: false, saveWorldObjects: true);

            if (!string.IsNullOrEmpty(spawnPointName) && GameManager.Instance != null)
            {
                GameManager.Instance.SetSceneTransitionMode(spawnPointName);
            }

            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            yield return FadeOut();

            currentSceneName = sceneName;

            yield return SceneManager.LoadSceneAsync(sceneName);


            // ⭐ 씬 로드 완료 후 약간 대기
            yield return new WaitForEndOfFrame();

            yield return FadeIn();

            IsSceneTransitioning = false;
            nextSpawnPointName = "";

        }

        // =========================
        // Fade
        // =========================

        private IEnumerator FadeIn()
        {
            if (fadeImage == null) yield break;

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
            if (fadeImage == null) yield break;

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
    }
}