using UnityEngine;

namespace FaintFear
{
    public class BGMTrigger : MonoBehaviour
    {
        public enum TriggerType
        {
            OnEnter,
            OnExit,
            OnEnable,
            OnDisable,
            Manual
        }

        [Header("Trigger")]
        public TriggerType triggerType = TriggerType.OnEnter;
        public string bgmName;
        public bool stopCurrentBGM;
        public bool oneTime = true;

        private bool triggered;

        private void OnEnable()
        {
            if (triggerType == TriggerType.OnEnable)
                Play();
        }

        private void OnDisable()
        {
            if (triggerType == TriggerType.OnDisable)
                Play();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerType != TriggerType.OnEnter) return;
            if (!other.CompareTag("Player")) return;

            Play();
        }

        private void OnTriggerExit(Collider other)
        {
            if (triggerType != TriggerType.OnExit) return;
            if (!other.CompareTag("Player")) return;

            Play();
        }

        public void Play()
        {
            if (oneTime && triggered) return;
            triggered = true;

            if (SoundManager.Instance == null) return;

            if (stopCurrentBGM)
                SoundManager.Instance.StopBGM();

            if (!string.IsNullOrEmpty(bgmName))
                SoundManager.Instance.PlayBGM(bgmName);
        }
    }
}
