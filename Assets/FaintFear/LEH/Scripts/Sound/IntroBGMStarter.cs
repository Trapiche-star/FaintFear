using FaintFear;
using UnityEngine;

public class IntroBGMStarter : MonoBehaviour
{
    void Start()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM("BGM_Calm", false);
        }
    }
}
