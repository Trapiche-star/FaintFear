using UnityEngine;
using FaintFear;

public class SceneBGMStarter : MonoBehaviour
{
    [Header("Play on Scene Start")]
    public string bgmName;                // 재생할 BGM 이름
    public bool rememberPrevious = false; // 이전 BGM 기억 여부

    void Start()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("[SceneBGMStarter] SoundManager가 씬에 존재하지 않습니다!");
            return;
        }

        if (string.IsNullOrEmpty(bgmName))
        {
            Debug.LogWarning("[SceneBGMStarter] BGM 이름이 비어있습니다!");
            return;
        }

        SoundManager.Instance.PlayBGM(bgmName, rememberPrevious);
        Debug.Log($"[SceneBGMStarter] BGM 재생 시작: {bgmName}");
    }
}
