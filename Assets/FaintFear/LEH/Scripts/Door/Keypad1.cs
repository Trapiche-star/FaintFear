using TMPro;
using UnityEngine;

namespace NavKeypad
{
    public class Keypad1 : MonoBehaviour
    {
        [Header("Password")]
        [SerializeField] private string correctCode = "2444";

        [Header("Door")]
        [SerializeField] private LockedSlidingDoor targetDoor;

        [Header("Display")]
        [SerializeField] private TMP_Text displayText;
        [SerializeField] private Renderer panelRenderer;

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.yellow;
        [SerializeField] private Color deniedColor = Color.red;
        [SerializeField] private Color grantedColor = Color.green;

        private string input = "";
        private bool unlocked;

        void Start()
        {
            displayText.text = "";
            panelRenderer.material.SetColor("_EmissionColor", normalColor);
        }

        public void AddInput(string value)
        {
            if (unlocked) return;

            if (value == "enter")
            {
                CheckPassword();
                return;
            }

            input += value;
            displayText.text = input;
        }

        void CheckPassword()
        {
            if (input == correctCode)
            {
                unlocked = true;
                displayText.text = "OK";
                panelRenderer.material.SetColor("_EmissionColor", grantedColor);
                targetDoor.UnlockDoor();
            }
            else
            {
                input = "";
                displayText.text = "ERR";
                panelRenderer.material.SetColor("_EmissionColor", deniedColor);
            }
        }
    }
}
