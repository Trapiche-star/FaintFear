/*using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace FaintFear
{
    public class HUDManager : MonoBehaviour
    {
        #region Variables
        private SceneFader fader;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            fader = GetComponentInChildren<SceneFader>();
            if (fader == null)
            {
                Debug.LogError("[HUDManager] SceneFader not found!");
                return;
            }

            // Panel 찾기 (비활성화된 것도 포함)
            if (fader.panelImage == null)
            {
                fader.panelImage = fader.GetComponentInChildren<Image>(true);
            }

            if (fader.panelImage != null)
            {
                // Panel GameObject 활성화
                fader.panelImage.gameObject.SetActive(true);

                // 시작 시 화면을 완전히 검은 상태로 고정
                Color c = fader.panelImage.color;
                c.a = 1f;
                fader.panelImage.color = c;

                Debug.Log($"[HUDManager] Screen initialized to black (alpha = {c.a})");
            }
            else
            {
                Debug.LogError("[HUDManager] Panel Image not found even with includeInactive!");
            }
        }

        private void Start()
        {
            // 한 프레임 대기 후 페이드 시작
            StartCoroutine(StartFadeAfterDelay());
        }

        private IEnumerator StartFadeAfterDelay()
        {
            // 한 프레임 대기
            yield return null;

            // 알파값 다시 확인
            if (fader != null && fader.panelImage != null)
            {
                Debug.Log($"[HUDManager] Before fade - Current alpha: {fader.panelImage.color.a}");
                FadeFromBlack();
            }
        }
        #endregion

        #region Custom Method
        public void FadeToBlack(Action onComplete = null)
        {
            if (fader != null)
            {
                Debug.Log("[HUDManager] FadeToBlack called");
                fader.FadeInToOne(onComplete);
            }
        }

        public void FadeFromBlack(Action onComplete = null)
        {
            if (fader != null)
            {
                Debug.Log("[HUDManager] FadeFromBlack called");
                fader.FadeOutToZero(onComplete);
            }
        }
        #endregion
    }
}*/