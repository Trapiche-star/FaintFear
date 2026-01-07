using UnityEngine;
using FaintFear;

public class SceneBGMStarter : MonoBehaviour
{
    [Header("Play on Scene Start")]
    public string bgmName;
    public bool rememberPrevious = false;

    void Start()
    {
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(bgmName))
        {
            SoundManager.Instance.PlayBGM(bgmName, rememberPrevious);
        }
    }
}
