using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    public GameObject crosshairBorder;

    public void ShowBorder()
    {
        crosshairBorder.SetActive(true);
    }

    public void HideBorder()
    {
        crosshairBorder.SetActive(false);
    }
}

