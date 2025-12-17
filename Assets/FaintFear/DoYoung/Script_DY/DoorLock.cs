using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 열쇠가 있어야 열리는 잠금 문
    /// PlayerStatus_DY의 열쇠 보유 여부를 검사하여 문을 열고 닫는다.
    /// </summary>
    public class DoorLock : Interactive
    {
        #region Variables

        private Transform hinge;            // 문이 회전하는 축
        private bool isMoving = false;      // 문이 현재 움직이는 중인지
        private bool isOpen = false;        // 문이 열려 있는지 여부

        [Header("잠금 설정")]
        [SerializeField] private bool isLocked = true;                  // 시작 시 잠금 여부
        [SerializeField] private RoomKeyType requiredKey = RoomKeyType.None; // 필요한 열쇠 타입

        [Header("UI 연결")]
        [SerializeField] private SequenceTextManager sequenceTextManager; // 문구 출력용
        [SerializeField] private ActionUI actionUI;                       // [E] 상호작용 UI
        [SerializeField] private float messageDuration = 2.0f;           // 문구 표시 시간

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            // 첫 번째 자식을 문 회전축으로 사용
            hinge = transform.GetChild(0);
        }

        #endregion


        #region Interaction Logic

        // 플레이어가 [E] 키로 상호작용할 때 호출
        public override void Interaction()
        {
            // 문이 이미 움직이고 있으면 입력 무시
            if (isMoving)
                return;

            // 플레이어 상태 싱글톤 가져오기
            PlayerStatus_DY player = PlayerStatus_DY.Instance;

            // 플레이어 상태가 없으면 중단
            if (player == null)
            {
                Debug.LogWarning("DoorLock: PlayerStatus_DY 인스턴스를 찾을 수 없습니다.");
                return;
            }

            // 문이 잠겨 있는 경우
            if (isLocked)
            {
                // 열쇠가 없으면 잠김 메시지 출력
                if (!player.HasKey(requiredKey))
                {
                    ShowSequenceMessage("문이 단단히 잠겨 있다.");
                    return;
                }

                // 열쇠가 있으면 잠금 해제
                isLocked = false;
                ShowSequenceMessage("열쇠로 잠금이 해제되었다.");
            }

            // 문 열기 / 닫기 처리
            if (!isOpen)
            {
                // 문 열기
                StartCoroutine(MoveDoorRoutine(-90f));
                actionUI?.ShowAction("문 닫기");
            }
            else
            {
                // 문 닫기
                StartCoroutine(MoveDoorRoutine(0f));
                actionUI?.ShowAction("문 열기");
            }

            // 문 상태 반전
            isOpen = !isOpen;
        }

        #endregion


        #region Door Animation

        // 문 회전 애니메이션 코루틴
        private IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true;

            float duration = 1.0f;   // 회전 시간
            float elapsed = 0f;      // 경과 시간

            Quaternion startRot = hinge.localRotation;
            Quaternion targetRot = Quaternion.Euler(0f, targetAngle, 0f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                hinge.localRotation = Quaternion.Lerp(startRot, targetRot, elapsed / duration);
                yield return null;
            }

            hinge.localRotation = targetRot;
            isMoving = false;
        }

        #endregion


        #region UI & Message

        // 시퀀스 텍스트 출력
        private void ShowSequenceMessage(string message)
        {
            if (sequenceTextManager == null)
                return;

            sequenceTextManager.gameObject.SetActive(true);
            sequenceTextManager.ShowMessage(message);

            StartCoroutine(HideSequenceAfterDelay());
        }

        // 일정 시간 후 시퀀스 텍스트 숨김
        private IEnumerator HideSequenceAfterDelay()
        {
            yield return new WaitForSeconds(messageDuration);

            if (sequenceTextManager != null)
                sequenceTextManager.gameObject.SetActive(false);
        }

        // 플레이어가 범위에 들어오면 [E] UI 표시
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                actionUI?.ShowAction("문 열기");
        }

        // 플레이어가 범위를 벗어나면 UI 숨김
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                actionUI?.HideAction();
        }

        #endregion
    }
}
