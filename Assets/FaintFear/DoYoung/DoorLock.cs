using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 열쇠가 있어야 열리는 잠금 문
    /// - PlayerStatus.hasKey를 검사
    /// - SequenceTextManager로 메시지 출력
    /// - ActionUI로 [E] 문 열기 / 닫기 표시
    /// </summary>
    public class DoorLock : Interactive
    {
        #region Variables
        private Transform hinge;        // 문이 회전할 축(자식 오브젝트)
        private bool isMoving = false;  // 문이 회전 중인지 여부
        private bool isOpen = false;    // 문이 열려 있는지 여부

        [Header("잠금 설정")]
        [SerializeField] private bool isLocked = true;           // 문이 잠긴 상태로 시작
        [SerializeField] private string requiredTag = "Player";  // 문을 열 수 있는 태그 (기본: Player)

        [Header("UI 연결 (수동 지정)")]
        [SerializeField] private SequenceTextManager sequenceTextManager; // 대사 출력용
        [SerializeField] private ActionUI actionUI;                       // [E] UI 표시용
        [SerializeField] private float messageDuration = 2.0f;            // 대사 표시 유지 시간
        #endregion


        #region Unity Events
        private void Awake()
        {
            // 문 회전축(hinge)을 첫 번째 자식으로 가정
            hinge = transform.GetChild(0);

            if (sequenceTextManager == null)
                Debug.LogWarning("DoorLock: SequenceTextManager가 인스펙터에 연결되지 않았습니다.");

            if (actionUI == null)
                Debug.LogWarning("DoorLock: ActionUI가 인스펙터에 연결되지 않았습니다.");
        }
        #endregion


        #region Interaction Logic
        /// <summary>
        /// 플레이어가 [E]를 눌러 상호작용 시 실행됨
        /// </summary>
        public override void Interaction()
        {
            if (isMoving) return; // 문이 움직이는 중이면 입력 무시

            // 플레이어 객체 찾기
            GameObject player = GameObject.FindGameObjectWithTag(requiredTag);
            if (player == null)
            {
                Debug.LogWarning("DoorLock: Player를 찾을 수 없습니다.");
                return;
            }

            // PlayerStatus 가져오기
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            if (status == null)
            {
                Debug.LogWarning("DoorLock: PlayerStatus가 존재하지 않습니다.");
                return;
            }

            // 🔒 잠긴 문이면 열쇠 보유 여부 확인
            if (isLocked)
            {
                if (!status.hasKey)
                {
                    ShowSequenceMessage("문이 단단히 잠겨 있다.");
                    return;
                }

                // 🔓 열쇠가 있을 경우
                isLocked = false;
                ShowSequenceMessage("열쇠로 잠금이 해제되었다.");
            }

            // 🔄 문 열기 / 닫기 실행
            if (!isOpen)
            {
                StartCoroutine(MoveDoorRoutine(-90f)); // 열기
                if (actionUI != null)
                    actionUI.ShowAction("문 닫기");
            }
            else
            {
                StartCoroutine(MoveDoorRoutine(0f)); // 닫기
                if (actionUI != null)
                    actionUI.ShowAction("문 열기");
            }

            isOpen = !isOpen;
        }
        #endregion


        #region Animation Routine
        /// <summary>
        /// 문 회전 애니메이션
        /// </summary>
        private IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true;

            float duration = 1.0f;
            float elapsed = 0f;

            Quaternion startRot = hinge.localRotation;
            Quaternion targetRot = Quaternion.Euler(0, targetAngle, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                hinge.localRotation = Quaternion.Lerp(startRot, targetRot, t);
                yield return null;
            }

            hinge.localRotation = targetRot;
            isMoving = false;
        }
        #endregion


        #region UI & Message
        /// <summary>
        /// 시퀀스 텍스트 출력
        /// </summary>
        private void ShowSequenceMessage(string message)
        {
            if (sequenceTextManager != null)
            {
                sequenceTextManager.gameObject.SetActive(true);
                sequenceTextManager.ShowMessage(message);
                StartCoroutine(HideSequenceAfterDelay());
            }
            else
            {
                Debug.Log($"[DoorLock] {message} (SequenceTextManager 미지정)");
            }
        }

        private IEnumerator HideSequenceAfterDelay()
        {
            yield return new WaitForSeconds(messageDuration);

            if (sequenceTextManager != null)
                sequenceTextManager.gameObject.SetActive(false);
        }

        /// <summary>
        /// 플레이어 근처 진입 시 [E] 문 열기 표시
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(requiredTag))
            {
                if (actionUI != null)
                    actionUI.ShowAction("문 열기");
            }
        }

        /// <summary>
        /// 플레이어 범위 이탈 시 UI 숨김
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(requiredTag))
            {
                if (actionUI != null)
                    actionUI.HideAction();
            }
        }
        #endregion
    }
}
