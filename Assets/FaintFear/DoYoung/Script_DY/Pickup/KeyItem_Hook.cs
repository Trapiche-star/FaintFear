using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 후크 보유 여부에 따라 획득 가능한 열쇠 아이템
    /// 후크가 없으면 획득 불가 및 시퀀스 텍스트를 출력한다
    /// </summary>
    public class KeyItem_Hook : PickupItemBase, IActionProvider
    {
        #region Variables

        [Header("Key Settings")]
        [SerializeField] private RoomKeyType keyType = RoomKeyType.None; // 획득 시 부여할 열쇠 타입

        [Header("Messages")]
        [SerializeField] private string needHookMessage = "후크가 필요해 보인다."; // 조건 미충족 시 출력
        [SerializeField] private string acquireMessage = "열쇠를 획득했다.";     // 획득 성공 시 출력

        [Header("Sequence")]
        [SerializeField] private SequenceTextManager sequenceTextManager; // 텍스트 출력과 시퀀스를 담당

        #endregion


        #region Unity Event Method

        // 참조 누락 여부를 사전에 확인한다
        private void Awake()
        {
            if (sequenceTextManager == null)
                Debug.LogWarning("[KeyItem_Hook] SequenceTextManager가 연결되지 않음");
        }

        #endregion


        #region Custom Method

        // 상호작용 시 후크 보유 여부를 검사한다
        public override void Interaction()
        {
            PuzzleInventory puzzleInventory = PuzzleInventory.Instance;
            if (puzzleInventory == null) return; // 만약 인벤토리가 없다면 이 메서드에서는 더 이상 상호작용하지 않는다

            if (!puzzleInventory.HasHook)        // 만약 후크가 없다면 획득을 차단한다
            {
                ShowMessage(needHookMessage);
                return;
            }

            base.Interaction();                  // 만약 후크가 있다면 기본 픽업 로직을 실행한다
        }

        // 실제 픽업 시 플레이어에게 열쇠를 부여한다
        protected override void OnPickup()
        {
            PlayerStatus playerStatus = PlayerStatus.Instance;
            if (playerStatus == null) return;    // 만약 플레이어 상태가 없다면 더 이상 처리하지 않는다

            if (keyType != RoomKeyType.None)     // 만약 유효한 열쇠 타입이라면 플레이어에게 지급한다
                playerStatus.AcquireKey(keyType);

            ShowMessage(acquireMessage);
        }

        // 메시지를 시퀀스 UI로 출력한다
        private void ShowMessage(string message)
        {
            if (sequenceTextManager == null) return; // 만약 시퀀스 매니저가 없다면 출력하지 않는다

            sequenceTextManager.gameObject.SetActive(true);
            sequenceTextManager.ShowMessage(message);
        }

        #endregion


        #region Property

        // 상호작용 UI에 표시될 액션 문구를 반환한다
        public string GetActionText()
        {
            return "[E] 열쇠 줍기";
        }

        #endregion
    }
}
