using UnityEngine;

namespace FaintFear
{
    public class ToggleSFX : MonoBehaviour
    {
        public string onSFX;
        public string offSFX;

        private bool lastState;

        public void Play(bool currentState)
        {
            if (currentState == lastState) return;

            lastState = currentState;

            string sfx = currentState ? onSFX : offSFX;

            if (!string.IsNullOrEmpty(sfx) && SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(sfx);
        }
    }
}
