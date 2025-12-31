using TMPro;
using UnityEngine;

public class PasswordDoorLock : MonoBehaviour
{
    [Header("Password")]
    [SerializeField] private string correctPassword = "2444";

    [Header("UI")]
    [SerializeField] private GameObject passwordCanvas;
    [SerializeField] private TMP_Text displayText;

    private string currentInput = "";
    private bool isOpen;

    void Start()
    {
        passwordCanvas.SetActive(false);
    }

    public void OpenUI()
    {
        if (isOpen) return;

        currentInput = "";
        displayText.text = "";
        passwordCanvas.SetActive(true);
        Time.timeScale = 0f; // 게임 정지
    }

    public void CloseUI()
    {
        passwordCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    // 숫자 버튼
    public void InputNumber(string num)
    {
        if (currentInput.Length >= 4) return;

        currentInput += num;
        displayText.text = currentInput;
    }

    // Clear 버튼
    public void Clear()
    {
        currentInput = "";
        displayText.text = "";
    }

    // Enter 버튼
    public void Enter()
    {
        if (currentInput == correctPassword)
        {
            Debug.Log("문 열림");
            isOpen = true;
            CloseUI();
            // 여기서 문 애니메이션 / Lock.Unlock() 호출
        }
        else
        {
            Debug.Log("비밀번호 틀림");
            Clear();
        }
    }
}
