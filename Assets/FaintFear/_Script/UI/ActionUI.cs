using UnityEngine;
using TMPro;

namespace FaintFear
{
    /// <summary>
    /// 상호작용 안내 텍스트 UI 제어
    /// </summary>
    public class ActionUI : MonoBehaviour
    {
        // 상호작용 문구를 표시할 TextMeshPro 텍스트
        [SerializeField] private TextMeshProUGUI actionText;

        private void Awake()
        {
            // 인스펙터에 연결되지 않았으면 자신의 TextMeshProUGUI를 자동으로 가져옴
            if (actionText == null)
                actionText = GetComponent<TextMeshProUGUI>();

            // 시작 시 상호작용 UI 숨김
            HideAction();
        }

        // 상호작용 문구 표시
        public void ShowAction(string text)
        {
            // "문 열기 [E]" 형태로 텍스트 설정
            actionText.text = text;

            // 텍스트 오브젝트 활성화
            actionText.gameObject.SetActive(true);
        }

        // 상호작용 문구 숨김
        public void HideAction()
        {
            // 텍스트 오브젝트 비활성화
            actionText.gameObject.SetActive(false);
        }
    }
}
