using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 후크 보유 여부에 따라 획득 가능한 열쇠 아이템
    /// 후크가 없으면 획득할 수 없으며 안내 시퀀스를 출력한다
    /// </summary>
    public class KeyItem_Hook : Interactive, IActionProvider
    {
        #region Variables

        [SerializeField] private RoomKeyType keyType = RoomKeyType.None;
        // 획득할 열쇠 타입

        [SerializeField] private string needHookMessage = "후크가 필요해 보인다.";
        // 후크가 없을 때 출력할 메시지

        [SerializeField] private string acquireMessage = "열쇠를 획득했다.";
        // 열쇠 획득 시 출력할 메시지

        [SerializeField] private SequenceTextManager sequenceTextManager;
        // 텍스트 출력과 시퀀스를 담당

        #endregion


        #region Unity Event Method

        private void Awake()
        {
        if (sequenceTextManager == null)
            Debug.LogWarning("KeyItem_후크: SequenceTextManager가 연결되지 않음");
        // 시퀀스 매니저 연결 여부를 확인한다
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            PuzzleInventory puzzleInventory = PuzzleInventory.Instance;
            if (puzzleInventory == null) return;
            // 만약 [퍼즐 인벤토리가 존재하지 않는다면] [상호작용을 중단한다]

            if (!puzzleInventory.HasBoltCutter)
            {
                if (sequenceTextManager != null)
                {
                    sequenceTextManager.gameObject.SetActive(true);
                    sequenceTextManager.ShowMessage(needHookMessage);
                }
                // 만약 [후크를 보유하지 않았다면] [안내 메시지를 출력하고 종료한다]

                return;
            }

            PlayerStatus playerStatus = PlayerStatus.Instance;
            if (playerStatus == null) return;
            // 만약 [플레이어 상태 정보가 없다면] [처리를 중단한다]

            if (keyType != RoomKeyType.None)
                playerStatus.AcquireKey(keyType);
            // 열쇠 타입이 유효하면 플레이어에게 열쇠를 추가한다

            if (sequenceTextManager != null)
            {
                sequenceTextManager.gameObject.SetActive(true);
                sequenceTextManager.ShowMessage(acquireMessage);
            }
            // 열쇠 획득 시 시퀀스 메시지를 출력한다

            Destroy(gameObject);
            // 열쇠 오브젝트를 제거한다
        }

        #endregion


        #region Property

        // Action UI에 표시할 문구 제공
        public string GetActionText()
        {
            return "열쇠 줍기";
            // 상호작용 UI에 표시될 텍스트를 반환한다
        }

        #endregion
    }
}
