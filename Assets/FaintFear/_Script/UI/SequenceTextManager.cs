using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using TMPro;

public class SequenceTextManager : MonoBehaviour
{
    public TextMeshProUGUI targetText;

    void Start()
    {
        if (targetText == null)
        {
            targetText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (targetText != null)
        {
            targetText.gameObject.SetActive(false);
        }
    }

    public void ShowMessage(string message)
    {
        if (targetText == null) return;

        targetText.text = message;

        targetText.gameObject.SetActive(true);

        StopAllCoroutines();

        StartCoroutine(DisableTimer());
    }

    IEnumerator DisableTimer()
    {
        yield return new WaitForSeconds(3.0f);

        targetText.gameObject.SetActive(false);
    }
}