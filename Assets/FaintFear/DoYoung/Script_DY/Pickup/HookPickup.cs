using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 후크 아이템 획득 처리
    /// 퍼즐 인벤토리에 후크(영구 도구)를 추가한다
    /// </summary>
    public class HookPickup : PickupItemBase, IActionProvider
    {
        [Header("Message")]
        [SerializeField] private string messageText = "후크를 획득했다.";

        [Header("Sequence")]
        [SerializeField] private SequenceTextManager sequenceTextManager;

        private void Awake()
        {
            if (sequenceTextManager == null)
                Debug.LogWarning("[HookPickup] SequenceTextManager가 연결되지 않음");
        }

        // ===================== Pickup =====================

        protected override void OnPickup()
        {
            PuzzleInventory inventory = PuzzleInventory.Instance;
            if (inventory == null) return;

            // 후크(영구 도구) 획득
            inventory.AcquireBoltCutter();

            // 메시지 출력
            if (sequenceTextManager != null)
            {
                sequenceTextManager.gameObject.SetActive(true);
                sequenceTextManager.ShowMessage(messageText);
            }
        }

        // ===================== UI =====================

        public string GetActionText()
        {
            return "[E] 줍기";
        }
    }
}
