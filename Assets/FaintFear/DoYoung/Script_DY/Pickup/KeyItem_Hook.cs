using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 후크 보유 여부에 따라 획득 가능한 열쇠 아이템
    /// 후크가 없으면 획득 불가 + 안내 메시지 출력
    /// </summary>
    public class KeyItem_Hook : PickupItemBase, IActionProvider
    {
        #region Variables

        [Header("Key Settings")]
        [SerializeField] private RoomKeyType keyType = RoomKeyType.None;

        [Header("Messages")]
        [SerializeField] private string needHookMessage = "후크가 필요해 보인다.";
        [SerializeField] private string acquireMessage = "열쇠를 획득했다.";

        [Header("Sequence")]
        [SerializeField] private SequenceTextManager sequenceTextManager;

        #endregion

        private void Awake()
        {
            if (sequenceTextManager == null)
                Debug.LogWarning("[KeyItem_Hook] SequenceTextManager가 연결되지 않음");
        }

        // ===================== Interaction =====================

        public override void Interaction()
        {
            PuzzleInventory puzzleInventory = PuzzleInventory.Instance;
            if (puzzleInventory == null) return;

            // ❌ 후크가 없으면 획득 불가
            if (!puzzleInventory.HasBoltCutter)
            {
                ShowMessage(needHookMessage);
                return;
            }

            // ⭕ 후크가 있으면 정상 픽업
            base.Interaction();
        }

        // ===================== Pickup =====================

        protected override void OnPickup()
        {
            PlayerStatus playerStatus = PlayerStatus.Instance;
            if (playerStatus == null) return;

            if (keyType != RoomKeyType.None)
                playerStatus.AcquireKey(keyType);

            ShowMessage(acquireMessage);
        }

        // ===================== UI =====================

        public string GetActionText()
        {
            return "[E] 열쇠 줍기";
        }

        // ===================== Helper =====================

        private void ShowMessage(string message)
        {
            if (sequenceTextManager == null) return;

            sequenceTextManager.gameObject.SetActive(true);
            sequenceTextManager.ShowMessage(message);
        }
    }
}