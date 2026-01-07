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
        [SerializeField] private SceneTransitionDoor targetDoorLock; // ⭐ 해제할 DoorLock

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
                // ✅ 정답 처리
                isUnlocked = true;

                if (displayText != null)
                    displayText.text = "OK";

                SetPanelColor(grantedColor);

                if (targetDoorLock != null)
                {
                    // 1️⃣ 문 해제
                    targetDoorLock.Unlock();

                    // 2️⃣ 런타임 상태 기록 (문 상태)
                    string doorID = targetDoorLock.GetInstanceID().ToString();
                    RuntimeStateManager.RecordDoorState(doorID, isOpen: true, isLocked: false);

                    // 3️⃣ 바로 체크포인트 저장
                    SaveSystem.SaveGame(
                        checkpointId: "auto_checkpoint_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                        tutorialCompleted: GameManager.TutorialCompleted,
                        saveWorldObjects: true
                    );

                    Debug.Log($"[UnifiedKeypad] 비밀번호 성공, 문 해제 및 자동저장 완료: {doorID}");
                }
            }
            else
            {
                // 오답
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