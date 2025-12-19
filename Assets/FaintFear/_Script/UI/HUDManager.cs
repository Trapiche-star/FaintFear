using UnityEngine;
using System.Collections;
using System;

namespace FaintFear
{
    /// <summary>
    /// HUD 텍스트 시퀀스와 화면 페이드 기능을 제공하는 도구
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        #region Variables

        [SerializeField] private SequenceTextManager textManager; // 텍스트 출력과 시퀀스를 담당
        private SceneFader fader;                                 // 화면 페이드 연출 담당

        #endregion


        #region Unity Event Method

        // HUD 매니저가 생성될 때 필요한 도구들을 준비한다
        private void Awake()
        {
            // HUD 하위 오브젝트에서 화면 페이드를 담당하는 SceneFader를 찾는다
            fader = GetComponentInChildren<SceneFader>();

            // 만약 텍스트 매니저가 아직 연결되지 않았을 때
            if (textManager == null)
                // 자식 오브젝트에 있는 SequenceTextManager를 찾아서 연결한다
                textManager = GetComponentInChildren<SequenceTextManager>();

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

        // 단일 문장을 HUD에 출력한다
        public void ShowDialogue(string message)
        {
            // 만약 텍스트 매니저가 null이 아닐 때 (즉, 존재할 때)
            if (textManager != null)
                // 전달받은 문장을 HUD에 출력한다
                textManager.ShowMessage(message);

            // 그렇지 않으면 아무 것도 하지 않고 넘어간다
        }

        // 여러 문장을 순서대로 출력하는 텍스트 시퀀스를 실행한다
        public IEnumerator ShowDialogueSequence(string[] lines, float holdTime)
        {
            // 만약 텍스트 매니저가 없다면 더 이상 진행할 수 없으므로 여기서 끝낸다
            if (textManager == null)
                yield break;

            // 그동안 전달받은 모든 문장을 하나씩 순서대로 반복한다
            foreach (string line in lines)
            {
                // 그래서 현재 문장을 HUD에 출력한다
                textManager.ShowMessage(line);

                // 그리고 지정된 시간만큼 화면에 유지되도록 기다린다
                yield return new WaitForSeconds(holdTime);
            }

            // 모든 문장 출력이 끝났으므로 텍스트를 숨긴다
            textManager.Hide();
        }

        #endregion
    }
}
