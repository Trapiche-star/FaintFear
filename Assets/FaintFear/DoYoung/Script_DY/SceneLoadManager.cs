using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace FaintFear
{
    /// <summary>
    /// 씬 이동을 전담하는 영속 관리자
    /// 다른 오브젝트로부터 씬 이동 요청을 받아 처리한다
    /// </summary>
    public class SceneLoadManager : MonoBehaviour
    {
        #region Singleton

        public static SceneLoadManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return; // 만약 [이미 존재한다면] [중복 생성을 막고 제거한다]
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 씬 전환 시에도 파괴되지 않도록 설정한다
        }

        #endregion


        #region Variables

        private string currentScene;                     // 현재 활성 씬 이름
        private Stack<string> sceneStack = new Stack<string>(); // 이전 씬 기록 스택
        private HashSet<string> loadedScenes = new HashSet<string>(); // 로드된 씬 목록

        #endregion


        #region Unity Event Method

        private void Start()
        {
            currentScene = SceneManager.GetActiveScene().name;
            loadedScenes.Add(currentScene);
            // 최초 시작 씬을 현재 씬으로 등록한다
        }

        #endregion


        #region Custom Method

        // 외부 오브젝트에서 씬 이동을 요청할 때 호출
        public void RequestMoveToScene(string targetSceneName)
        {
            if (string.IsNullOrEmpty(targetSceneName)) return;
            // 만약 [씬 이름이 비어 있다면] [요청을 무시한다]

            if (targetSceneName == currentScene) return;
            // 만약 [이미 현재 씬이라면] [이동하지 않는다]

            sceneStack.Push(currentScene);
            // 현재 씬을 이전 씬 스택에 저장한다

            LoadSceneInternal(targetSceneName);
            // 실제 씬 로드 처리를 실행한다
        }

        // 이전 씬으로 돌아가는 요청
        public void RequestReturnToPreviousScene()
        {
            if (sceneStack.Count == 0) return;
            // 만약 [돌아갈 씬이 없다면] [요청을 무시한다]

            string previousScene = sceneStack.Pop();
            // 가장 최근의 씬을 꺼낸다

            LoadSceneInternal(previousScene);
            // 이전 씬으로 이동한다
        }

        // 실제 씬 로드 공통 처리
        private void LoadSceneInternal(string sceneName)
        {
            if (!loadedScenes.Contains(sceneName))
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                loadedScenes.Add(sceneName);
                // 만약 [아직 로드되지 않은 씬이라면] [Additive로 로드한다]
            }

            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid()) return;
            // 만약 [씬이 유효하지 않다면] [처리를 중단한다]

            SceneManager.SetActiveScene(scene);
            // 해당 씬을 활성 씬으로 설정한다

            currentScene = sceneName;
            // 현재 씬 정보를 갱신한다
        }

        #endregion


        #region Property

        public string CurrentScene => currentScene;
        // 현재 활성 씬 이름을 반환한다

        public bool HasPreviousScene => sceneStack.Count > 0;
        // 이전 씬이 존재하는지 여부를 반환한다

        #endregion
    }
}
