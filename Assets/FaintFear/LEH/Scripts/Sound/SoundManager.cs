using UnityEngine;
using System.Collections.Generic;

namespace FaintFear
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        public Sound[] bgms;
        public Sound[] sfxs;

        private Dictionary<string, Sound> bgmDict;
        private Dictionary<string, Sound> sfxDict;

        private Sound currentBGM;

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

            foreach (var s in bgms)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                
                s.source.loop = true; //+ 반복 재생 가능하게

                bgmDict[s.name] = s;
            }

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

        //BGM
        public void PlayBGM(string name)
        {
            if (!bgmDict.ContainsKey(name)) return;

            if (currentBGM != null && currentBGM.name == name)
                return;

            if (currentBGM != null)
                currentBGM.source.Stop();

            currentBGM = bgmDict[name];
            currentBGM.source.Play();
        }

        public void StopBGM()
        {
            if (currentBGM != null)
                currentBGM.source.Stop();

            currentBGM = null;
        }

        //SFX
        public void PlaySFX(string name)
        {
            if (!sfxDict.ContainsKey(name)) return;
            sfxDict[name].source.PlayOneShot(sfxDict[name].clip);
        }
    }
}

