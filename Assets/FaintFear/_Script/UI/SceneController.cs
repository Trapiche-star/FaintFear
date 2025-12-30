using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using FaintFear;

public class SceneController : MonoBehaviour
{
    public SceneFader sceneFader;
    public SimpleBGMPlayer bgmPlayer;

    [SerializeField] private float Duration = 8f;
    public string nextSceneName;

    private void Start()
    {
        sceneFader.FadeOutToZero(() =>
        {
            StartCoroutine(IntroFlow());
        });
    }

    private IEnumerator IntroFlow()
    {
        yield return new WaitForSeconds(Duration);

        // 1. 화면 페이드 아웃
        sceneFader.FadeInToOne(() =>
        {
            // 2. 음악 페이드 아웃
            bgmPlayer.StopBGM();

            // 3. 음악 페이드 끝날 때까지 대기
            StartCoroutine(LoadNextSceneAfterBGM());
        });
    }

    private IEnumerator LoadNextSceneAfterBGM()
    {
        yield return new WaitForSeconds(bgmPlayer.fadeOutTime);
        SceneManager.LoadScene(nextSceneName);
    }
}
