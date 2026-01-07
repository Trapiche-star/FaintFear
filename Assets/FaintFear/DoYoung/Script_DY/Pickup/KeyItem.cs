using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 열쇠 아이템 획득 처리
    /// </summary>
    public class KeyItem : PickupItemBase, IActionProvider
    {
        [Header("Key Settings")]
        [SerializeField] private RoomKeyType keyType = RoomKeyType.None;

        [Header("Message")]
        [SerializeField] private string messageText = "이걸로 저쪽 문을 열 수 있을지도 모른다.";

        [Header("Sequence")]
        [SerializeField] private SequenceTextManager sequenceTextManager;

        private void Awake()
        {
            if (sequenceTextManager == null)
                Debug.LogWarning("[KeyItem] SequenceTextManager가 연결되지 않음");
        }

        // ===================== Pickup =====================

        protected override void OnPickup()
        {
            PlayerStatus playerStatus = PlayerStatus.Instance;
            if (playerStatus == null) return;

            if (keyType != RoomKeyType.None)
                playerStatus.AcquireKey(keyType);

            if (sequenceTextManager != null)
            {
                sequenceTextManager.gameObject.SetActive(true);
                sequenceTextManager.ShowMessage(messageText);
            }

            // + SFX 재생: 열쇠 획득 시 매번 재생
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("SFX_GetKey");
        }

        // ===================== UI =====================

        public string GetActionText()
        {
            return "[E] 열쇠 줍기";
        }
    }
}
