using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace FaintFear
{
    public class SceneController : MonoBehaviour
    {
        public SceneFader sceneFader;
        public SimpleBGMPlayer bgmPlayer;
        public UISlideShowFade slideShow;
        public string nextSceneName;

        private void Start()
        {
            // 인트로 시작 시 화면 페이드 아웃 (밝아짐)
            sceneFader.FadeStart(0f);

            // 슬라이드 쇼 종료 이벤트 등록
            slideShow.onSlideShowFinished += OnSlideShowFinished;
        }

        private void OnSlideShowFinished()
        {
            // BGM 페이드 아웃 시작 (백그라운드에서 실행)
            if (bgmPlayer != null)
            {
                bgmPlayer.StopBGM();
            }

            // 화면 페이드 아웃 후 바로 씬 전환
            StartCoroutine(FadeOutAndLoadScene());
        }

        private IEnumerator FadeOutAndLoadScene()
        {
            // 화면 페이드 아웃 (어두워짐)
            float fadeTime = 1f;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / fadeTime;
                float a = sceneFader.curve.Evaluate(t);
                sceneFader.img.color = new Color(0f, 0f, 0f, a);
                yield return null;
            }

            // 페이드 완료 후 바로 씬 전환 (BGM 대기 없음)
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
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