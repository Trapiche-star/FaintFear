using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 인트로/슬라이드쇼 씬 컨트롤러
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        [Header("Components")]
        public SimpleBGMPlayer bgmPlayer;
        public UISlideShowFade slideShow;

        [Header("Scene Settings")]
        public string nextSceneName = "Level01";
        public string spawnPointName = ""; // 필요시 스폰 포인트 지정

        private void Start()
        {
            // 슬라이드 쇼 종료 이벤트 등록
            if (slideShow != null)
            {
                slideShow.onSlideShowFinished += OnSlideShowFinished;
            }
        }

        private void OnSlideShowFinished()
        {
            // BGM 페이드 아웃 시작 (백그라운드에서 실행)
            if (bgmPlayer != null)
            {
                bgmPlayer.StopBGM();
            }

            // SceneLoadManager로 씬 전환
            if (SceneLoadManager.Instance != null)
            {
                if (string.IsNullOrEmpty(spawnPointName))
                {
                    SceneLoadManager.Instance.LoadScene(nextSceneName);
                }
                else
                {
                    SceneLoadManager.Instance.LoadScene(nextSceneName, spawnPointName);
                }
            }
            else
            {
                Debug.LogError("[SceneController] SceneLoadManager not found!");
                // 폴백: 일반 씬 전환
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
        }

        private void OnDestroy()
        {
            // 이벤트 해제
            if (slideShow != null)
            {
                slideShow.onSlideShowFinished -= OnSlideShowFinished;
            }
        }
    }
}