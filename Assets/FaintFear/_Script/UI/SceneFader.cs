using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace FaintFear
{
    /// <summary>
    /// 화면의 알파값을 변경하여 페이드 연출을 수행하는 도구
    /// </summary>
    public class SceneFader : MonoBehaviour
    {
        public Image panelImage;

        private void Awake()  
        {
            if (panelImage == null)
            {
                // ⭐ 비활성화된 오브젝트도 찾도록 true 파라미터 추가
                panelImage = GetComponentInChildren<Image>(true);

                if (panelImage != null)
                {
                    Debug.Log("[SceneFader] Panel Image found (including inactive)");

                    // ⭐ Panel을 활성화
                    panelImage.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("[SceneFader] Panel Image not found!");
                }
            }
        }

        // 화면을 밝게 만든다 (알파 1 → 0)
        public void FadeOutToZero(Action onComplete = null)
        {
            if (panelImage == null)
            {
                Debug.LogError("[SceneFader] FadeOutToZero - panelImage is null!");
                return;
            }

            StopAllCoroutines();
            StartCoroutine(FadeAlpha(1f, 0f, onComplete));
        }

        // 화면을 검게 만든다 (알파 0 → 1)
        public void FadeInToOne(Action onComplete = null)
        {
            if (panelImage == null)
            {
                Debug.LogError("[SceneFader] FadeInToOne - panelImage is null!");
                return;
            }

            StopAllCoroutines();
            StartCoroutine(FadeAlpha(0f, 1f, onComplete));
        }

        // 알파값을 시간에 따라 보간하는 공용 코루틴
        private IEnumerator FadeAlpha(float startAlpha, float endAlpha, Action onComplete)
        {
            float elapsedTime = 0f;
            float duration = 1.0f;

            Color c = panelImage.color;
            c.a = startAlpha;
            panelImage.color = c;

            Debug.Log($"[SceneFader] FadeAlpha started: {startAlpha} → {endAlpha}");

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);

                c.a = newAlpha;
                panelImage.color = c;

                yield return null;
            }

            // 최종 알파값 보정
            c.a = endAlpha;
            panelImage.color = c;

            Debug.Log($"[SceneFader] FadeAlpha completed: alpha = {endAlpha}");

            onComplete?.Invoke();
        }
    }
}