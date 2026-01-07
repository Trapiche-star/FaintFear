using UnityEngine;

public class IdleBGMManager : MonoBehaviour
{
    [Header("Idle BGM Settings")]
    public AudioClip idleClip;       // Inspector에서 넣는 상시 BGM
    [Range(0f, 1f)]
    public float volume = 0.5f;      // 볼륨 조절

    [Header("Check Settings")]
    public float checkInterval = 0.2f; // 다른 소리 체크 간격

    private AudioSource idleBGM;     // 실제 재생용 AudioSource
    private float timer = 0f;

    private static IdleBGMManager instance;

    void Awake()
    {
        // 싱글톤 처리 (씬 전환 시 유지)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // AudioSource 생성
            idleBGM = gameObject.AddComponent<AudioSource>();
            idleBGM.clip = idleClip;
            idleBGM.loop = true;
            idleBGM.playOnAwake = false;
            idleBGM.volume = volume;
        }
        else
        {
            Destroy(gameObject); // 이미 존재하면 중복 제거
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;
            UpdateIdleBGM();
        }
    }

    void UpdateIdleBGM()
    {
        bool isOtherPlaying = false;

        // 최신 Unity 기준: FindObjectsOfType 대신 FindObjectsByType 사용
        AudioSource[] allSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource src in allSources)
        {
            if (src != idleBGM && src.isPlaying)
            {
                isOtherPlaying = true;
                break;
            }
        }

        if (isOtherPlaying)
        {
            if (idleBGM.isPlaying)
                idleBGM.Pause();
        }
        else
        {
            if (!idleBGM.isPlaying)
                idleBGM.Play();
        }
    }
    // 외부에서 강제로 IdleBGM 재생/정지 가능
    public void PlayIdleBGM()
    {
        if (!idleBGM.isPlaying)
            idleBGM.Play();
    }

    public void StopIdleBGM()
    {
        if (idleBGM.isPlaying)
            idleBGM.Stop();
    }

}




