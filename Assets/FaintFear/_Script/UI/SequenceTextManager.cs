using UnityEngine;
using UnityEngine.UI; // Text 컴포넌트를 사용하기 위해 필수
using System.Collections;
using TMPro;

public class SequenceTextManager : MonoBehaviour
{
    // 제어할 자식 텍스트 컴포넌트
    public TextMeshProUGUI targetText;

    void Start()
    {
        // 인스펙터에 할당하지 않았다면 자식 오브젝트에서 자동으로 찾습니다.
        if (targetText == null)
        {
            targetText = GetComponentInChildren<TextMeshProUGUI>();
        }

        // 게임 시작 시에는 일단 안 보이게 꺼둡니다.
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
        // 1초 대기
        yield return new WaitForSeconds(3.0f);

        // 오브젝트 비활성화 (안 보이게 하기)
        targetText.gameObject.SetActive(false);
    }
}