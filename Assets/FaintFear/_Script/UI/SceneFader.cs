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
        void Start()
        {
            // 만약 페이드 이미지가 아직 연결되지 않았다면
            if (panelImage == null)
            {
                // 자식 오브젝트에서 Image 컴포넌트를 찾아서 연결한다
                panelImage = GetComponentInChildren<Image>();
            }

            // 여기서는 알파값을 건드리지 않는다
            // → 시작 알파 상태는 Image 인스펙터 값에 의존한다
        }

        // 화면을 밝게 만든다 (알파 1 → 0)
        public void FadeOutToZero(Action onComplete = null)
        {
            // 만약 페이드 이미지가 존재한다면
            if (panelImage != null)
            {
                // 진행 중인 페이드가 있다면 중단하고
                StopAllCoroutines();

                // 검은 화면에서 점점 투명해지는 페이드를 시작한다
                StartCoroutine(FadeAlpha(1f, 0f, onComplete));
            }
        }

        // 화면을 검게 만든다 (알파 0 → 1)
        public void FadeInToOne(Action onComplete = null)
        {
            // 만약 페이드 이미지가 존재한다면
            if (panelImage != null)
            {
                // 진행 중인 페이드가 있다면 중단하고
                StopAllCoroutines();

                // 밝은 화면에서 점점 검어지는 페이드를 시작한다
                StartCoroutine(FadeAlpha(0f, 1f, onComplete));
            }
        }

        // 알파값을 시간에 따라 보간하는 공용 코루틴
        IEnumerator FadeAlpha(float startAlpha, float endAlpha, Action onComplete)
        {
            float elapsedTime = 0f;   // 경과 시간
            float duration = 1.0f;   // 페이드에 걸리는 시간

            // 시작 알파값을 즉시 적용한다
            Color c = panelImage.color;
            c.a = startAlpha;
            panelImage.color = c;

            // 그동안 페이드 시간이 끝날 때까지 반복한다
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime; // 프레임마다 시간 누적

                // 시작 알파 → 목표 알파로 부드럽게 보간한다
                float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                c.a = newAlpha;
                panelImage.color = c;

                // 다음 프레임까지 대기한다
                yield return null;
            }

            // 페이드가 끝나면 정확한 목표 알파값으로 고정한다
            c.a = endAlpha;
            panelImage.color = c;

            // 만약 완료 콜백이 있다면 호출한다
            if (onComplete != null)
            {
                onComplete.Invoke();
            }
        }
    }
}
