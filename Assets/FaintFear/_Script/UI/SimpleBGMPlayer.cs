using UnityEngine;
using System.Collections;

public class SimpleBGMPlayer : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip bgmClip;

    [Header("Settings")]
    public bool playOnStart = true;
    public bool loop = false;
    public float volume = 1f;

    [Header("Fade (Optional)")]
    public float fadeInTime = 0f;
    public float fadeOutTime = 0f;

    private void Start()
    {
        if (audioSource == null || bgmClip == null)
            return;

        audioSource.clip = bgmClip;
        audioSource.loop = loop;
        audioSource.volume = (fadeInTime > 0f) ? 0f : volume;

        if (playOnStart)
        {
            audioSource.Play();

            if (fadeInTime > 0f)
                StartCoroutine(FadeIn());
        }
    }

    public void StopBGM()
    {
        if (!audioSource.isPlaying) return;

        // 페이드 아웃 시작 볼륨을 보정
        audioSource.volume = volume;

        if (fadeOutTime > 0f)
            StartCoroutine(FadeOut());
        else
            audioSource.Stop();
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < fadeInTime)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, volume, time / fadeInTime);
            yield return null;
        }
        audioSource.volume = volume;
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < fadeOutTime)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeOutTime);
            yield return null;
        }

        audioSource.Stop();
    }
}
