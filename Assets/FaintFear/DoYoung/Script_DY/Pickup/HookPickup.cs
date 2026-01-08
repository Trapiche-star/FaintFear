using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 후크 아이템 획득 처리
    /// 퍼즐 인벤토리에 후크(영구 도구)를 추가한다
    /// </summary>
    public class HookPickup : PickupItemBase, IActionProvider
    {
        #region Variables

        [Header("Message")]
        [SerializeField] private string messageText = "후크를 획득했다."; // 획득 시 출력 메시지

        [Header("Sequence")]
        [SerializeField] private SequenceTextManager sequenceTextManager; // 텍스트 출력과 시퀀스를 담당

        #endregion


        #region Unity Event Method

        // 참조 누락 여부를 사전에 확인한다
        private void Awake()
        {
            if (sequenceTextManager == null)
                Debug.LogWarning("[HookPickup] SequenceTextManager가 연결되지 않음");
        }

        #endregion


        #region Custom Method

        // 실제 픽업 시 퍼즐 인벤토리에 후크를 등록한다
        protected override void OnPickup()
        {
            PuzzleInventory inventory = PuzzleInventory.Instance;
            if (inventory == null) return;    // 만약 인벤토리가 없다면 더 이상 처리하지 않는다

            inventory.AcquireHook();          // 만약 아직 후크가 없다면 후크를 획득 처리한다

            if (sequenceTextManager != null)  // 만약 시퀀스 매니저가 존재한다면 메시지를 출력한다
            {
                sequenceTextManager.gameObject.SetActive(true);
                sequenceTextManager.ShowMessage(messageText);
            }
        }

        #endregion


        #region Property

        // 상호작용 UI에 표시될 액션 문구를 반환한다
        public string GetActionText()
        {
            return "[E] 줍기";
        }

        #endregion
    }
}
