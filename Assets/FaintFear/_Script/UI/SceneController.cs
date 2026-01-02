using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using FaintFear;

public class SceneController : MonoBehaviour
{
    public SceneFader sceneFader;
    public SimpleBGMPlayer bgmPlayer;
    public UISlideShowFade slideShow;

    public string nextSceneName;

    private void Start()
    {
        // 인트로 시작 시 화면 페이드 아웃
        sceneFader.FadeOutToZero(() =>
        {
            // 슬라이드 쇼 종료
            slideShow.onSlideShowFinished += OnSlideShowFinished;
        });
    }

    private void OnSlideShowFinished()
    {
        // 화면 페이드 인
        sceneFader.FadeInToOne(() =>
        {
            // 음악 페이드 아웃
            bgmPlayer.StopBGM();

            // 음악 페이드 종료 후 씬 전환
            StartCoroutine(LoadNextSceneAfterBGM());
        });
    }

    private IEnumerator LoadNextSceneAfterBGM()
    {
        yield return new WaitForSeconds(bgmPlayer.fadeOutTime);
        SceneManager.LoadScene(nextSceneName);
    }
}
