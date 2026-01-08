using UnityEngine;
using UnityEngine.UI;
using FaintFear;

public class UIButtonSFXManager : MonoBehaviour
{
    [Header("재생할 버튼 클릭 SFX 이름")]
    public string buttonSFXName = "SFX_ButtonDown";

    void Start()
    {
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(() =>
            {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySFX(buttonSFXName);
            });
        }
    }
}
