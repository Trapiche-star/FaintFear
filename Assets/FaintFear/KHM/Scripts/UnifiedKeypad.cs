using UnityEngine;
using TMPro;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 키패드 상호작용 - 비밀번호 입력 후 DoorLock 해제
    /// </summary>
    public class UnifiedKeypad : MonoBehaviour
    {
        #region Variables

        [Header("Password")]
        [SerializeField] private string correctCode = "2444";
        [SerializeField] private int maxLength = 4;

        [Header("Door Lock")]
        [SerializeField] private LockedDoorBase targetDoor; // ⭐ 해제할 DoorLock

        [Header("Display")]
        [SerializeField] private TMP_Text displayText;
        [SerializeField] private Renderer panelRenderer;
        [SerializeField] private string emissionProperty = "_EmissionColor";

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.yellow;
        [SerializeField] private Color deniedColor = Color.red;
        [SerializeField] private Color grantedColor = Color.green;

        [Header("Messages")]
        [SerializeField] private SequenceTextManager sequenceText;

        [Header("Timing")]
        [SerializeField] private float errorDisplayTime = 1.5f;

        private string input = "";
        private bool isUnlocked = false;

        #endregion

        #region Unity Event Methods

        private void Start()
        {
            ResetDisplay();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// KeypadButton에서 호출
        /// </summary>
        public void AddInput(string value)
        {
            if (isUnlocked) return;

            if (value == "enter" || value == "Enter")
            {
                CheckPassword();
                return;
            }

            if (input.Length >= maxLength) return;

            input += value;
            UpdateDisplay();
        }

        public void ClearInput()
        {
            if (isUnlocked) return;
            input = "";
            UpdateDisplay();
        }

        #endregion

        #region Private Methods

        private void UpdateDisplay()
        {
            if (displayText != null)
            {
                displayText.text = input;
            }
        }

        private void CheckPassword()
        {
            if (input == correctCode)
            {
                isUnlocked = true;

                if (displayText != null)
                    displayText.text = "OK";

                SetPanelColor(grantedColor);

                if (targetDoor != null)
                {
                    // 🔓 DoorLock / ChainLock / 기타 LockedDoorBase 전부 해제
                    targetDoor.ForceUnlockFromKeypad();

                    Debug.Log($"[UnifiedKeypad] 키패드로 문 잠금 해제: {targetDoor.GetID()}");
                }
            }
            else
            {
                StartCoroutine(ShowErrorRoutine());
            }
        }

        private IEnumerator ShowErrorRoutine()
        {
            if (displayText != null)
                displayText.text = "ERR";

            SetPanelColor(deniedColor);

            if (sequenceText != null)
            {
                sequenceText.ShowMessage("비밀번호가 틀렸다.");
            }

            yield return new WaitForSeconds(errorDisplayTime);

            input = "";
            ResetDisplay();
        }

        private void ResetDisplay()
        {
            if (displayText != null)
                displayText.text = "";

            SetPanelColor(normalColor);
        }

        private void SetPanelColor(Color color)
        {
            if (panelRenderer != null && panelRenderer.material != null)
            {
                panelRenderer.material.SetColor(emissionProperty, color);
            }
        }

        #endregion
    }
}