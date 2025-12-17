using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 열쇠 아이템 획득 처리
    /// </summary>
    public class KeyItem : Interactive, IActionProvider
    {
        // 이 열쇠의 타입
        [SerializeField] private RoomKeyType keyType = RoomKeyType.None;

        // 획득 시 출력할 메시지
        [SerializeField] private string messageText = "이걸로 저쪽 문을 열 수 있을지도 모른다.";

        // 메시지 표시 시간
        [SerializeField] private float messageDuration = 2.0f;

        // 시퀀스 텍스트 출력용
        [SerializeField] private SequenceTextManager sequenceTextManager;

        private void Awake()
        {
            // 시퀀스 텍스트 연결 확인
            if (sequenceTextManager == null)
                Debug.LogWarning("KeyItem: SequenceTextManager가 연결되지 않음");
        }

        // E 키 상호작용 시 호출
        public override void Interaction()
        {
            // 플레이어 상태 가져오기
            PlayerStatus playerStatus = PlayerStatus.Instance;
            if (playerStatus == null)
                return;

            // 열쇠 타입이 유효하면 플레이어에게 추가
            if (keyType != RoomKeyType.None)
                playerStatus.AcquireKey(keyType);

            // 획득 메시지 출력
            if (sequenceTextManager != null)
            {
                sequenceTextManager.gameObject.SetActive(true);
                sequenceTextManager.ShowMessage(messageText);
                StartCoroutine(HideMessageAfterDelay());
            }

            // 열쇠 오브젝트 제거
            Destroy(gameObject);
        }

        // 일정 시간 후 메시지 숨김
        private System.Collections.IEnumerator HideMessageAfterDelay()
        {
            yield return new WaitForSeconds(messageDuration);

            if (sequenceTextManager != null)
                sequenceTextManager.gameObject.SetActive(false);
        }

        // ActionUI에 표시할 문구 제공
        public string GetActionText()
        {
            return "열쇠 줍기";
        }
    }
}
