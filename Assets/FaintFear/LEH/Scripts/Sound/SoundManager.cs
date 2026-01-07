using UnityEngine;
using System.Collections.Generic;

namespace FaintFear
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        [Header("Sound Lists")]
        public Sound[] bgms;
        public Sound[] sfxs;

        private Dictionary<string, Sound> bgmDict;
        private Dictionary<string, Sound> sfxDict;

        private Sound currentBGM;
        private Sound previousBGM;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }

        private void Init()
        {
            bgmDict = new Dictionary<string, Sound>();
            sfxDict = new Dictionary<string, Sound>();

            // ================= BGM =================
            foreach (var s in bgms)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = true;
                s.source.playOnAwake = false;

                bgmDict[s.name] = s;
            }

            // ================= SFX =================
            foreach (var s in sfxs)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = false;
                s.source.playOnAwake = false;

                sfxDict[s.name] = s;
            }
        }

        // ================= BGM =================
        public void PlayBGM(string name, bool rememberPrevious = true)
        {
            if (!bgmDict.ContainsKey(name))
            {
                Debug.LogWarning($"[SoundManager] BGM not found: {name}");
                return;
            }

            if (currentBGM != null && currentBGM.name == name)
                return;

            if (rememberPrevious)
                previousBGM = currentBGM;

            if (currentBGM != null)
                currentBGM.source.Stop();

            currentBGM = bgmDict[name];
            currentBGM.source.Play();
        }

        public void ResumePreviousBGM()
        {
            if (previousBGM == null)
                return;

            if (currentBGM != null)
                currentBGM.source.Stop();

            currentBGM = previousBGM;
            previousBGM = null;
            currentBGM.source.Play();
        }

        // ❌ 무음 방지
        public void StopBGM()
        {
            Debug.LogWarning("[SoundManager] StopBGM 사용 금지");
        }

        // ================= SFX =================
        public void PlaySFX(string name)
        {
            if (!sfxDict.ContainsKey(name))
            {
                Debug.LogWarning($"[SoundManager] SFX not found: {name}");
                return;
            }

            sfxDict[name].source.PlayOneShot(sfxDict[name].clip);
        }
    }
}
