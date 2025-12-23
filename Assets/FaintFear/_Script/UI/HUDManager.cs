using UnityEngine;
using System.Collections;
using System;

namespace FaintFear
{
    /// <summary>
    /// 페이드 기능을 제공하는 도구
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        #region Variables
        private SceneFader fader;                                 // 화면 페이드 연출 담당
        #endregion

        #region Unity Event Method

        // HUD 매니저가 생성될 때 필요한 도구들을 준비한다
        private void Awake()
        {
            // HUD 하위 오브젝트에서 화면 페이드를 담당하는 SceneFader를 찾는다
            fader = GetComponentInChildren<SceneFader>();

            // 시작 시 화면을 완전히 검은 상태로 고정한다
            if (fader != null && fader.panelImage != null) // 페이더와 페이드 이미지가 존재할 때
            {
                Color c = fader.panelImage.color; // 현재 페이드 이미지의 색상을 가져오고
                c.a = 1f;                         // 알파값을 1로 만들어 화면을 완전히 가린다
                fader.panelImage.color = c;       // 변경된 색상을 다시 적용한다
            }
        }
        #endregion

        #region Custom Method

        // 화면을 검게 만든다 (알파 0 → 1)
        public void FadeToBlack(Action onComplete = null)
        {
            // 만약 페이더가 존재한다면 화면을 검게 만든다
            if (fader != null)
                fader.FadeInToOne(onComplete);

            // 그렇지 않으면 아무 일도 하지 않는다
        }

        // 화면을 밝게 만든다 (알파 1 → 0)
        public void FadeFromBlack(Action onComplete = null)
        {
            // 만약 페이더가 존재한다면 화면을 밝게 만든다
            if (fader != null)
                fader.FadeOutToZero(onComplete);

            // 그렇지 않으면 아무 일도 하지 않는다
        }
        #endregion
    }
}
