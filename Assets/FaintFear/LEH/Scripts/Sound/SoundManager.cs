using UnityEngine;
using System;
using System.Collections.Generic;

namespace FaintFear
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        [Header("All Sounds")]
        public Sound[] sounds;

        AudioSource bgmSource;
        AudioSource sfxSource;

        void Awake()
        {
            // 싱글톤
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // AudioSource 2개 생성
            bgmSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();

            bgmSource.loop = true;
            sfxSource.loop = false;
        }

        // =========================
        // SFX 재생
        // =========================
        public void PlaySFX(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogWarning($"[SoundManager] SFX not found : {name}");
                return;
            }

            sfxSource.pitch = s.pitch;
            sfxSource.volume = s.volume;
            sfxSource.PlayOneShot(s.clip);
        }

        // =========================
        // BGM 재생
        // =========================
        public void PlayBGM(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogWarning($"[SoundManager] BGM not found : {name}");
                return;
            }

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
        }
    }
}

