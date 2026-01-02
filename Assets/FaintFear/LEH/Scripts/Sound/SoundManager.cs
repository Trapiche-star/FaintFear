using UnityEngine;
using System;

namespace FaintFear
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        [Header("Sound Data")]
        public Sound[] sounds;

        private AudioSource bgmSource;
        private AudioSource sfxSource;

        private void Awake()
        {
            //안전한 싱글톤 (Assertion 방지 핵심)
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // AudioSource 2개만 사용
            bgmSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();

            bgmSource.loop = true;
            sfxSource.loop = false;
        }

        // ======================
        // SFX
        // ======================
        public void PlaySFX(string soundName)
        {
            Sound s = Array.Find(sounds, x => x.name == soundName);

            if (s == null || s.clip == null)
            {
                Debug.LogWarning($"[SoundManager] SFX not found: {soundName}");
                return;
            }

            sfxSource.volume = s.volume;
            sfxSource.pitch = s.pitch;
            sfxSource.PlayOneShot(s.clip);
        }

        // ======================
        // BGM
        // ======================
        public void PlayBGM(string soundName)
        {
            Sound s = Array.Find(sounds, x => x.name == soundName);

            if (s == null || s.clip == null)
            {
                Debug.LogWarning($"[SoundManager] BGM not found: {soundName}");
                return;
            }

            // 같은 BGM이면 재실행 안 함
            if (bgmSource.clip == s.clip) return;

            bgmSource.Stop();
            bgmSource.clip = s.clip;
            bgmSource.volume = s.volume;
            bgmSource.pitch = s.pitch;
            bgmSource.loop = s.loop;
            bgmSource.Play();
        }

        public void StopBGM()
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        }
    }
}
