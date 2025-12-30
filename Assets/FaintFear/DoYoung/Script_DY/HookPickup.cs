using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 후크 아이템 획득 처리
    /// 상호작용을 통해 후크를 인벤토리에 추가하고 시퀀스를 출력한다
    /// </summary>
    public class HookPickup : Interactive, IActionProvider
    {
        #region Variables

        [SerializeField] private string messageText = "후크를 획득했다.";
        // 획득 시 출력할 시퀀스 메시지

        [SerializeField] private SequenceTextManager sequenceTextManager;
        // 텍스트 출력과 시퀀스를 담당

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            if (sequenceTextManager == null)
                Debug.LogWarning("HookPickup: SequenceTextManager가 연결되지 않음");
            // 시퀀스 매니저 연결 여부를 확인한다
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            PuzzleInventory inventory = PuzzleInventory.Instance;
            if (inventory == null) return;
            // 만약 [퍼즐 인벤토리가 존재하지 않는다면] [상호작용을 중단한다]

            inventory.AcquireBoltCutter();
            // 퍼즐 인벤토리에 후크(영구 도구)를 추가한다

            if (sequenceTextManager != null)
            {
                sequenceTextManager.gameObject.SetActive(true);
                sequenceTextManager.ShowMessage(messageText);
            }
            // 후크 획득 시 시퀀스 메시지를 출력한다

            Destroy(gameObject);
            // 후크 오브젝트를 제거한다
        }

        #endregion


        #region Property

        // Action UI에 표시할 문구 제공
        public string GetActionText()
        {
            return "줍기";
            // 상호작용 UI에 표시될 텍스트를 반환한다
        }

        #endregion
    }
}
