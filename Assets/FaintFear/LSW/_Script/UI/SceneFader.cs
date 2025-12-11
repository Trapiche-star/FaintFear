using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace FaintFear
{
    public class SceneFader : MonoBehaviour
    {
        public Image panelImage;

        void Start()
        {
            if (panelImage == null)
            {
                panelImage = GetComponentInChildren<Image>();
            }
        }

        // [화면 밝아짐] 알파값 1 -> 0 (투명해짐)
        public void FadeOutToZero(Action onComplete = null)
        {
            if (panelImage != null)
            {
                StopAllCoroutines();
                StartCoroutine(FadeAlpha(1f, 0f, onComplete));
            }
        }

        // [추가됨: 화면 어두워짐] 알파값 0 -> 1 (불투명해짐/검게 변함)
        public void FadeInToOne(Action onComplete = null)
        {
            if (panelImage != null)
            {
                StopAllCoroutines();
                StartCoroutine(FadeAlpha(0f, 1f, onComplete));
            }
        }

        // 공용 코루틴
        IEnumerator FadeAlpha(float startAlpha, float endAlpha, Action onComplete)
        {
            float elapsedTime = 0f;
            float duration = 1.0f;

            Color c = panelImage.color;
            c.a = startAlpha;
            panelImage.color = c;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                c.a = newAlpha;
                panelImage.color = c;
                yield return null;
            }

            c.a = endAlpha;
            panelImage.color = c;

            if (onComplete != null)
            {
                onComplete.Invoke();
            }
        }
    }
}

