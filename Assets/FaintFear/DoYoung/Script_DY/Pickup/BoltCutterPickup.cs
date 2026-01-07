using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 볼트 커터를 획득하는 퍼즐용 픽업 오브젝트
    /// 영구 퍼즐 도구로 등록되며 저장된다
    /// </summary>
    public class BoltCutterPickup : PickupItemBase, IActionProvider
    {
        [Header("UI")]
        [SerializeField] private SequenceTextManager sequenceText;

        [SerializeField] private string acquireMessage = "볼트 커터를 획득했다.";
        [SerializeField] private string alreadyHaveMessage = "이미 볼트 커터를 가지고 있다.";

        // ===================== Unity =====================

        private void Awake()
        {
            if (sequenceText == null)
                Debug.LogWarning($"{name}: SequenceTextManager가 지정되지 않음");
        }

        // ===================== Pickup =====================

        protected override void OnPickup()
        {
            PuzzleInventory inventory = PuzzleInventory.Instance;
            if (inventory == null) return;

            // 이미 보유 중이라면 메시지만 출력하고 종료
            if (inventory.HasBoltCutter)
            {
                ShowHUDMessage(alreadyHaveMessage);
                return;
            }

            // 볼트 커터 획득 처리
            inventory.AcquireBoltCutter();
            ShowHUDMessage(acquireMessage);
        }

        // ===================== UI =====================

        private void ShowHUDMessage(string message)
        {
            if (sequenceText == null) return;
            sequenceText.ShowMessage(message);
        }

        public string GetActionText()
        {
            return "[E] 줍기";
        }
    }
}
