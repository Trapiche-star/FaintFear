using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace FaintFear
{
    /// <summary>
    /// 씬 이동과 씬 상태 관리를 담당하는 영속 관리자
    /// Additive 로드 후 이전 씬을 비활성화한다
    /// </summary>
    public class SceneLoadManager : MonoBehaviour
    {
        #region Singleton

        public static SceneLoadManager Instance { get; private set; }

        #endregion


        #region Variables

        // 현재 활성 씬 이름
        private string currentSceneName;

        // 이전 씬 이름
        private string previousSceneName;

        // 로드된 씬 목록
        private HashSet<string> loadedScenes = new HashSet<string>();

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
                // 만약 [이미 인스턴스가 존재한다면] [중복 생성을 방지한다]
            }

            Instance = this;
            // 이 객체를 SceneLoadManager 싱글톤으로 등록한다

            DontDestroyOnLoad(gameObject);
            // 씬 이동 시에도 파괴되지 않도록 설정한다

            currentSceneName = SceneManager.GetActiveScene().name;
            loadedScenes.Add(currentSceneName);
            // 최초 시작 씬을 현재 씬으로 등록한다
        }

        #endregion


        #region Public Method

        // 외부 오브젝트에서 씬 이동을 요청할 때 호출된다
        public void RequestMoveToScene(string targetSceneName)
        {
            if (string.IsNullOrEmpty(targetSceneName)) return;
            // 만약 [씬 이름이 비어 있다면] [이 메서드에서는 더 이상 처리하지 않는다]

            if (targetSceneName == currentSceneName) return;
            // 만약 [이미 현재 씬이라면] [중복 이동을 방지한다]

            StartCoroutine(LoadSceneRoutine(targetSceneName));
            // 씬 로드 루틴을 시작한다
        }

        #endregion


        #region Custom Method

        // 씬을 Additive로 로드하고 이전 씬을 비활성화한다
        private IEnumerator LoadSceneRoutine(string targetSceneName)
        {
            previousSceneName = currentSceneName;
            // 이동 전 씬을 이전 씬으로 기록한다

            // 아직 로드되지 않은 씬이라면 Additive 로드
            if (!loadedScenes.Contains(targetSceneName))
            {
                yield return SceneManager.LoadSceneAsync(
                    targetSceneName,
                    LoadSceneMode.Additive
                );

                loadedScenes.Add(targetSceneName);
                // 로드된 씬 목록에 추가한다
            }

            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            if (!targetScene.IsValid()) yield break;
            // 만약 [씬이 유효하지 않다면] [이 메서드에서는 더 이상 처리하지 않는다]

            SceneManager.SetActiveScene(targetScene);
            // 새 씬을 활성 씬으로 설정한다

            DisableScene(previousSceneName);
            // 이전 씬을 화면에서 보이지 않게 비활성화한다

            currentSceneName = targetSceneName;
            // 현재 씬 정보를 갱신한다
        }

        // 특정 씬의 모든 루트 오브젝트를 비활성화한다
        private void DisableScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            // 만약 [씬 이름이 없다면] [이 메서드에서는 더 이상 처리하지 않는다]

            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid()) return;
            // 만약 [씬이 유효하지 않다면] [비활성화하지 않는다]

            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                roots[i].SetActive(false);
                // 이전 씬의 모든 루트 오브젝트를 비활성화한다
            }
        }

        // 다시 돌아올 때 사용할 수 있는 씬 활성화 메서드
        public void EnableScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            // 만약 [씬 이름이 없다면] [이 메서드에서는 더 이상 처리하지 않는다]

            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid()) return;
            // 만약 [씬이 유효하지 않다면] [활성화하지 않는다]

            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                roots[i].SetActive(true);
                // 해당 씬의 모든 루트 오브젝트를 다시 활성화한다
            }
        }

        #endregion
    }
}
