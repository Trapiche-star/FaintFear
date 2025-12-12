using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 열쇠 아이템 획득 처리
    /// 플레이어의 PlayerInteraction 시스템을 통해 상호작용됨
    /// </summary>
    public class KeyItem : Interactive
    {
        #region Variables
        [Header("획득 후 메시지 설정")]
        [SerializeField] private string messageText = "이걸로 저쪽 문을 열 수 있을지도 모른다."; // 시퀀스 메시지
        [SerializeField] private float messageDuration = 2.0f; // 메시지 유지 시간

        [Header("UI 연결 (수동 지정)")]
        [SerializeField] private SequenceTextManager sequenceTextManager; // 대사 텍스트 출력용
        [SerializeField] private ActionUI actionUI; // Press [E] UI 표시용
        #endregion

        #region Unity Event
        private void Awake()
        {
            // 자동 탐색 대신 경고만 출력 (씬에서 수동 연결 권장)
            if (sequenceTextManager == null)
                Debug.LogWarning("PickupKey: SequenceTextManager가 인스펙터에 연결되어 있지 않습니다.");

            if (actionUI == null)
                Debug.LogWarning("PickupKey: ActionUI가 인스펙터에 연결되어 있지 않습니다.");
        }
        #endregion

        #region Custom Method
        /// <summary>
        /// 플레이어가 상호작용(E) 키를 눌렀을 때 실행됨
        /// </summary>
        public override void Interaction()
        {
            // 플레이어 객체 찾기
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("PickupKey: Player를 찾을 수 없습니다.");
                return;
            }

            // PlayerStatus 또는 다른 인벤토리 매니저를 통해 열쇠 보유 상태 설정
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            if (status != null)
                status.hasKey = true;
            else
                Debug.Log("PickupKey: PlayerStatus가 없어서 hasKey를 저장하지 못했습니다.");

            // 대사 텍스트 출력
            if (sequenceTextManager != null)
            {
                sequenceTextManager.gameObject.SetActive(true);
                sequenceTextManager.ShowMessage(messageText);
                player.GetComponent<MonoBehaviour>().StartCoroutine(HideMessageAfterDelay());
            }

            // 액션 UI 숨김
            if (actionUI != null)
                actionUI.HideAction();

            // 아이템 제거
            Destroy(gameObject);

            Debug.Log("열쇠를 획득했습니다!");
        }

        /// <summary>
        /// 일정 시간 후 시퀀스 텍스트 자동 비활성화
        /// </summary>
        private System.Collections.IEnumerator HideMessageAfterDelay()
        {
            yield return new WaitForSeconds(messageDuration);

            if (sequenceTextManager != null)
                sequenceTextManager.gameObject.SetActive(false);
        }
        #endregion
    }
}
