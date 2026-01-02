using UnityEngine;

namespace FaintFear
{
    public class BGMTrigger : MonoBehaviour
    {
        [Header("재생할 BGM 목록")]
        public string[] bgmNames;

        [Header("옵션")]
        public bool playRandom = false;
        public bool preventDuplicate = true;

        private string lastPlayed;

        //이벤트에 연결
        public void PlayBGM()
        {
            if (SoundManager.Instance == null || bgmNames.Length == 0)
                return;

            string bgm;

            if (playRandom)
                bgm = bgmNames[Random.Range(0, bgmNames.Length)];
            else
                bgm = bgmNames[0];

            if (preventDuplicate && bgm == lastPlayed)
                return;

            lastPlayed = bgm;
            SoundManager.Instance.PlayBGM(bgm);
        }

        public void StopBGM()
        {
            if (SoundManager.Instance == null) return;
            SoundManager.Instance.StopBGM();
            lastPlayed = null;
        }
    }
}
