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
        private Sound previousBGM;   // ⭐ 이벤트 전 BGM 저장

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

            // ---------- BGM ----------
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

            // ---------- SFX ----------
            foreach (var s in sfxs)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = false;

                sfxDict[s.name] = s;
            }
        }

        // ==================================================
        // BGM
        // ==================================================

        /// <summary>
        /// BGM 재생
        /// rememberPrevious = true → 이벤트용 (끝나면 복귀)
        /// rememberPrevious = false → 기본 상시 BGM
        /// </summary>
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

        /// <summary>
        /// 이벤트 종료 후 이전 BGM으로 복귀
        /// (무음 절대 발생 안 함)
        /// </summary>
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

        // ⚠️ 이 프로젝트에서는 사용 금지 (무음 방지)
        public void StopBGM()
        {
            Debug.LogWarning("[SoundManager] StopBGM은 이 프로젝트에서 사용하지 않습니다.");
        }

        // ==================================================
        // SFX
        // ==================================================
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
