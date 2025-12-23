using TMPro;
using UnityEngine;

public class PasswordDoorLock : MonoBehaviour
{
    [Header("Password")]
    [SerializeField] private string correctPassword = "2444";

    [Header("Target Lock")]
    [SerializeField] private Lock targetLock;

    [Header("UI")]
    [SerializeField] private GameObject passwordUI;
    [SerializeField] private TMP_InputField inputField;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    bool playerInside;
    bool unlocked;

    void Start()
    {
        if (passwordUI != null)
            passwordUI.SetActive(false);
    }

    void Update()
    {
        if (!playerInside || unlocked) return;

        if (Input.GetKeyDown(interactKey))
        {
            passwordUI.SetActive(true);
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    // 버튼 OnClick에 연결
    public void SubmitPassword()
    {
        if (inputField.text == correctPassword)
        {
            unlocked = true;

            //도어락 방식으로만 잠금 해제
            targetLock.UnlockByDoorLock();

            passwordUI.SetActive(false);
            gameObject.SetActive(false); // 도어락 사용 종료
        }
        else
        {
            inputField.text = "";
            Debug.Log("비밀번호가 틀렸습니다.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (passwordUI != null)
                passwordUI.SetActive(false);
        }
    }
}