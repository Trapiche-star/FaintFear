using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UISlideShowFade : MonoBehaviour
{
    [Header("UI References")]
    public Image imageA;
    public Image imageB;
    public TMP_Text displayText;

    [Header("Slide Data")]
    public Sprite[] images;
    [TextArea]
    public string[] texts;

    [Header("Settings")]
    [SerializeField] private int slideCount = 2;
    [SerializeField] private float interval = 3f;
    [SerializeField] private float fadeDuration = 1f;

    // 슬라이드 쇼 종료 
    public Action onSlideShowFinished;

    private bool usingImageA = true;

    private void Start()
    {
        imageA.color = new Color(1, 1, 1, 1);
        imageB.color = new Color(1, 1, 1, 0);

        StartCoroutine(SlideRoutine());
    }

    private IEnumerator SlideRoutine()
    {
        int count = Mathf.Min(slideCount, images.Length, texts.Length);

        // 첫 슬라이드 세팅
        imageA.sprite = images[0];
        displayText.text = texts[0];

        for (int i = 1; i < count; i++)
        {
            yield return new WaitForSeconds(interval);
            yield return StartCoroutine(CrossFade(images[i], texts[i]));
        }

        // 마지막 슬라이드 유지 시간
        yield return new WaitForSeconds(interval);

        // 슬라이드 쇼 종료 알림
        onSlideShowFinished?.Invoke();
    }

    private IEnumerator CrossFade(Sprite nextSprite, string nextText)
    {
        Image from = usingImageA ? imageA : imageB;
        Image to = usingImageA ? imageB : imageA;

        to.sprite = nextSprite;
        displayText.text = nextText;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            from.color = new Color(1, 1, 1, 1 - t);
            to.color = new Color(1, 1, 1, t);

            yield return null;
        }

        from.color = new Color(1, 1, 1, 0);
        to.color = new Color(1, 1, 1, 1);

        usingImageA = !usingImageA;
    }
}
