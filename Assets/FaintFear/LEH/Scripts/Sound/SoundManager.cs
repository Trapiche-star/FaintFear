using UnityEngine;

namespace FaintFear
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        [Header("All Sounds")]
        public Sound[] sounds;

        [Header("BGM")]
        public string currentBGM;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
                s.source.playOnAwake = s.playOnAwake;

                if (s.playOnAwake)
                    s.source.Play();
            }
        }

        //일반 재생
        public void Play(string name)
        {
            Sound s = GetSound(name);
            if (s == null) return;

            s.source.Play();
        }

        //정지
        public void Stop(string name)
        {
            Sound s = GetSound(name);
            if (s == null) return;

            s.source.Stop();
        }

        //BGM 교체 (기존 BGM 정지 후 새 BGM 재생)
        public void PlayBGM(string name)
        {
            if (currentBGM == name) return;

            if (!string.IsNullOrEmpty(currentBGM))
                Stop(currentBGM);

            currentBGM = name;
            Play(name);
        }

        private Sound GetSound(string name)
        {
            Sound s = System.Array.Find(sounds, sound => sound.name == name);

            if (s == null)
                Debug.LogWarning($"Sound not found: {name}");

            return s;
        }
    }
}
