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
        public Image panelImage; // 화면을 덮는 페이드용 이미지

        // SceneFader가 활성화될 때 한 번 호출된다
        private void Start()
        {
            // 만약 페이드 이미지가 아직 연결되지 않았다면
            if (panelImage == null)
            {
                // 자식 오브젝트에서 Image 컴포넌트를 찾아서 연결한다
                panelImage = GetComponentInChildren<Image>();
            }

            // 시작 알파값은 HUDManager에서 제어한다
            // 여기서는 알파값을 건드리지 않는다
        }

        // 화면을 밝게 만든다 (알파 1 → 0)
        public void FadeOutToZero(Action onComplete = null)
        {
            if (panelImage == null) return;

            StopAllCoroutines();
            StartCoroutine(FadeAlpha(1f, 0f, onComplete));
        }

        // 화면을 검게 만든다 (알파 0 → 1)
        public void FadeInToOne(Action onComplete = null)
        {
            if (panelImage == null) return;

            StopAllCoroutines();
            StartCoroutine(FadeAlpha(0f, 1f, onComplete));
        }

        // 알파값을 시간에 따라 보간하는 공용 코루틴
        private IEnumerator FadeAlpha(float startAlpha, float endAlpha, Action onComplete)
        {
            float elapsedTime = 0f; // 경과 시간
            float duration = 1.0f; // 페이드 지속 시간

            Color c = panelImage.color;
            c.a = startAlpha;
            panelImage.color = c;

            while (elapsedTime < duration)
            {
                /* 기존 코드 (게임 시간 기준)
                elapsedTime += Time.deltaTime;
                */

                // 수정 코드 (연출용, Time.timeScale 무시)
                elapsedTime += Time.unscaledDeltaTime;

                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                c.a = newAlpha;
                panelImage.color = c;

                Debug.Log($"FadeAlpha running: {panelImage.color.a}");

                yield return null;
            }

            // 최종 알파값 보정
            c.a = endAlpha;
            panelImage.color = c;

            onComplete?.Invoke();
        }
    }
}
