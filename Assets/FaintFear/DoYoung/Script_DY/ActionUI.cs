using UnityEngine;
using TMPro;

namespace FaintFear
{
    /// <summary>
    /// "Press [E]" 같은 상호작용 텍스트 제어
    /// </summary>
    public class ActionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI actionText;

        private void Awake()
        {
            if (actionText == null)
                actionText = GetComponent<TextMeshProUGUI>();

            HideAction();
        }

        public void ShowAction(string text)
        {
            actionText.text = $"Press [E] {text}";
            actionText.gameObject.SetActive(true);
        }

        public void HideAction()
        {
            actionText.gameObject.SetActive(false);
        }
    }
}
