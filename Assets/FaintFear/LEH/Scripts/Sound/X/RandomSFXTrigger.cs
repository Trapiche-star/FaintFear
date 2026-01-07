using UnityEngine;

namespace FaintFear
{
    public class RandomSFXTrigger : MonoBehaviour
    {
        [Header("Random SFX List")]
        public string[] sfxNames;

        [Header("Option")]
        public bool oneShot = false;

        private bool played;

        public void Play()
        {
            if (oneShot && played) return;
            if (sfxNames == null || sfxNames.Length == 0) return;

            played = true;

            int index = Random.Range(0, sfxNames.Length);
            string sfx = sfxNames[index];

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(sfx);
        }
    }
}
