using UnityEngine;
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

    // 단일 문장을 즉시 출력한다
    public void ShowMessage(string message)
    {
        // 만약 텍스트가 없다면 여기서 끝낸다
        if (targetText == null) return;

        // 전달받은 문장을 설정한다
        targetText.text = message;

        // 텍스트를 화면에 표시한다
        targetText.gameObject.SetActive(true);

        // 이전 타이머가 있다면 중단한다
        StopAllCoroutines();

        // 자동 숨김 타이머를 시작한다
        StartCoroutine(DisableTimer());
    }

    // 일정 시간이 지나면 텍스트를 숨긴다
    IEnumerator DisableTimer()
    {
        // 그동안 3초를 기다린다
        yield return new WaitForSeconds(3.0f);

        // 그래서 텍스트를 비활성화한다
        Hide();
    }

    // 텍스트를 즉시 숨긴다
    public void Hide()
    {
        // 만약 텍스트가 없다면 아무 것도 하지 않는다
        if (targetText == null) return;

        // 실행 중인 코루틴을 모두 중단한다
        StopAllCoroutines();

        // 텍스트를 화면에서 숨긴다
        targetText.gameObject.SetActive(false);
    }
}
