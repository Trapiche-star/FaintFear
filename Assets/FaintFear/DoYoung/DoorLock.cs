using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 잠금 기능이 포함된 문 제어
    /// 일반 문(Door) 로직 + 열쇠 필요 여부 + 시퀀스 텍스트 + 액션 UI 출력
    /// </summary>
    public class DoorLock : Interactive
    {
        #region Variables
        private Transform hinge;        // 문이 회전할 축(자식 오브젝트)
        private bool isMoving = false;  // 문이 회전 중인지 여부
        private bool isOpen = false;    // 문이 열려 있는지 여부

        [Header("잠금 설정")]
        [SerializeField] private bool isLocked = true;          // 문이 잠긴 상태로 시작할지 여부
        [SerializeField] private string requiredTag = "Player"; // 문을 열 수 있는 대상의 태그

        [Header("UI 연결")]
        [SerializeField] private SequenceTextManager sequenceTextManager; // 시퀀스 텍스트 매니저
        [SerializeField] private ActionUI actionUI;                       // 액션 UI (Press [E])
        [SerializeField] private float messageDuration = 2.0f;            // 메시지 유지 시간 (초)
        #endregion

        #region Unity Event
        private void Awake()
        {
            // 문 회전축(hinge)을 자식 오브젝트로부터 가져오기
            hinge = transform.GetChild(0);

            // 자동 탐색 (수동 연결 안 되어 있으면)
            if (sequenceTextManager == null)
                sequenceTextManager = FindObjectOfType<SequenceTextManager>();

            if (actionUI == null)
                actionUI = FindObjectOfType<ActionUI>();
        }
        #endregion

        #region Custom Methods
        public override void Interaction()
        {
            // 문이 움직이는 중이면 입력 무시
            if (isMoving) return;

            // 문이 잠겨 있으면 열쇠 확인
            if (isLocked)
            {
                GameObject player = GameObject.FindGameObjectWithTag(requiredTag);
                KeyPickup key = player != null ? player.GetComponent<KeyPickup>() : null;

                // 열쇠가 없을 때
                if (key == null || !key.hasKey)
                {
                    ShowSequenceMessage("문이 단단히 잠겨 있다.");
                    return;
                }

                // 열쇠가 있을 때 → 잠금 해제
                isLocked = false;
                ShowSequenceMessage("열쇠로 잠금이 해제되었다.");
            }

            // 문 열기/닫기
            if (!isOpen)
            {
                StartCoroutine(MoveDoorRoutine(-90f));
                if (actionUI != null)
                    actionUI.ShowAction("문 닫기"); // 문 열린 상태에서는 "닫기"로 변경
            }
            else
            {
                StartCoroutine(MoveDoorRoutine(0f));
                if (actionUI != null)
                    actionUI.ShowAction("문 열기");
            }

            // 상태 반전
            isOpen = !isOpen;
        }

        private IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true;

            float duration = 1.0f;
            float elapsedTime = 0f;

            Quaternion startRotation = hinge.localRotation;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                hinge.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
                yield return null;
            }

            hinge.localRotation = targetRotation;
            isMoving = false;
        }

        /// <summary>
        /// 시퀀스 텍스트 출력 (문 잠김 안내 등)
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
                Debug.LogWarning($"SequenceTextManager가 없습니다. 콘솔에 출력: {message}");
            }
        }

        private IEnumerator HideSequenceAfterDelay()
        {
            yield return new WaitForSeconds(messageDuration);

            if (sequenceTextManager != null)
                sequenceTextManager.gameObject.SetActive(false);
        }

        /// <summary>
        /// 플레이어가 문 근처에 있을 때 ActionUI 표시 (Press [E] 문 열기)
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
        /// 플레이어가 범위 벗어나면 ActionUI 숨김
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
