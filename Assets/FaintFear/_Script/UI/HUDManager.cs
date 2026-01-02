using UnityEngine;
using UnityEngine.UI;
using System;

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

            // ⭐ Panel이 비활성화되어 있으면 활성화
            if (fader.panelImage == null)
            {
                fader.panelImage = fader.GetComponentInChildren<Image>(true);
            }

            if (fader.panelImage != null)
            {
                // ⭐ Panel GameObject 활성화
                fader.panelImage.gameObject.SetActive(true);

                // ⭐ 시작 시 화면을 완전히 검은 상태로 고정
                Color c = fader.panelImage.color;
                c.a = 1f;
                fader.panelImage.color = c;

                Debug.Log("[HUDManager] Screen initialized to black (alpha = 1)");
            }
            else
            {
                Debug.LogError("[HUDManager] Panel Image not found even with includeInactive!");
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
}