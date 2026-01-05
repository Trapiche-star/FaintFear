using UnityEngine;

namespace FaintFear
{
    public class SFXTrigger : MonoBehaviour
    {
        public string sfxName;
        public bool oneShot = true;
        private bool played;

        public void Play()
        {
            if (oneShot && played) return;
            played = true;

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(sfxName);
        }
    }
}
