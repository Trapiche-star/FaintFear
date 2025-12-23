using UnityEngine;
using System.Collections;
using TMPro;
public class SequenceTextManager : MonoBehaviour
{
    public TextMeshProUGUI targetText;
    private Coroutine currentTimer;

    void Start()
    {
        if (targetText == null)
            targetText = GetComponentInChildren<TextMeshProUGUI>();

        targetText.gameObject.SetActive(false);
    }
    // 단일 문장을 즉시 출력한다
    public void ShowMessage(string message, float duration = 3f)
    {
        if (targetText == null) return;

        targetText.text = message;
        targetText.gameObject.SetActive(true);

        if (currentTimer != null)
            StopCoroutine(currentTimer);

        currentTimer = StartCoroutine(DisableTimer(duration));
    }

    // 여러 문장을 순서대로 출력하는 텍스트 시퀀스를 실행한다
    public IEnumerator ShowDialogueSequence(string[] lines, float holdTime)
    {
        // 그동안 전달받은 모든 문장을 하나씩 순서대로 반복한다
        foreach (string line in lines)
        {
            // 그래서 현재 문장을 HUD에 출력한다
            ShowMessage(line);

            // 그리고 지정된 시간만큼 화면에 유지되도록 기다린다
            yield return new WaitForSeconds(holdTime);
        }

        // 모든 문장 출력이 끝났으므로 텍스트를 숨긴다
        Hide();
    }

    // 조건 만족 전까지 유지되는 메시지
    public void ShowPersistentMessage(string message)
    {
        if (targetText == null) return;

        if (currentTimer != null)
        {
            StopCoroutine(currentTimer);
            currentTimer = null;
        }

        targetText.text = message;
        targetText.gameObject.SetActive(true);
    }

    // 텍스트를 즉시 숨긴다
    public void Hide()
    {
        if (targetText == null) return;

        if (currentTimer != null)
        {
            StopCoroutine(currentTimer);
            currentTimer = null;
        }

        targetText.gameObject.SetActive(false);
    }

    // 일정 시간이 지나면 텍스트를 숨긴다
    IEnumerator DisableTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Hide();
    }
}