using UnityEngine;
using UnityEngine.UI;

namespace FaintFear
{
    /// <summary>
    /// HUD 매니저
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        // 텍스트 매니저를 연결하기 위한 변수
        public SequenceTextManager textManager;

        void Start()
        {
            Transform targetObj = transform.GetChild(1);
            Image targetImage = targetObj.GetChild(0).GetComponent<Image>();
            SceneFader fader = targetObj.GetComponent<SceneFader>();

            Color tempColor = targetImage.color;
            tempColor.a = 1f;
            targetImage.color = tempColor;

            if (textManager == null)
                textManager = transform.GetChild(0).GetComponent<SequenceTextManager>();

            fader.FadeOutToZero(() =>
            {
                if (textManager != null)
                {
                    textManager.ShowMessage("Where i am?");
                }
            });
        }

        void Update()
        {

        }
    }
}
