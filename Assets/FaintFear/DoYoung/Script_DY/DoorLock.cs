using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 열쇠 또는 키패드 조건에 따라 잠긴 문을 열고 닫는 상호작용 도어
    /// </summary>
    public class DoorLock : Interactive, IActionProvider
    {
        #region Variables

        private Transform hinge;                  // 문 회전을 담당하는 힌지 트랜스폼
        private bool isMoving = false;             // 문 애니메이션 진행 여부
        private bool isOpen = false;               // 문 개방 상태

        [Header("Door State")]
        [SerializeField] private bool interactionEnabled = true; // 도어 상호작용 가능 여부
        [SerializeField] private bool isLocked = true;            // 문 잠금 상태
        [SerializeField] private bool useKeypadLock = false;      // 키패드 잠금 문 여부

        [Header("Key Settings")]
        [SerializeField] private RoomKeyType requiredKey;         // 문 개방에 필요한 열쇠 타입

        [Header("Sequence")]
        [SerializeField] private SequenceTextManager sequenceText; // HUD 시퀀스 출력 관리자

        [Header("Custom Sequences")]
        [SerializeField, TextArea] private string lockedSequence;        // 열쇠 미보유 시 출력
        [SerializeField, TextArea] private string unlockedSequence;      // 잠금 해제 시 출력
        [SerializeField, TextArea] private string keypadSequence;        // 키패드 문 안내
        [SerializeField, TextArea] private string cannotUseKeySequence;  // 열쇠 사용 실패 시 출력

        #endregion


        #region Unity Event Method

        // 문 힌지 초기화
        private void Awake()
        {
            // 문 모델의 첫 번째 자식을 힌지로 사용한다
            hinge = transform.GetChild(0);
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용 입력 처리
        public override void Interaction()
        {
            if (!interactionEnabled) return;
            // 도어 상호작용이 비활성화된 경우 처리하지 않는다

            if (isMoving) return;
            // 문 애니메이션 중 중복 입력을 방지한다

            PlayerStatus player = PlayerStatus.Instance;
            if (player == null) return;
            // 플레이어 상태 정보를 가져올 수 없으면 처리하지 않는다

            if (isLocked && useKeypadLock)
            {
                ShowSequence(keypadSequence, "키패드로 잠금을 풀 수 있다.");
                return;
                // 키패드로 잠긴 문은 안내 시퀀스만 출력한다
            }

            if (isLocked)
            {
                if (!player.HasKey(requiredKey))
                {
                    ShowSequence(lockedSequence, "문이 단단히 잠겨 있다.");
                    return;
                    // 필요한 열쇠가 없으면 잠김 메시지를 출력한다
                }

                if (!player.ConsumeKey(requiredKey))
                {
                    ShowSequence(cannotUseKeySequence, "열쇠를 사용할 수 없다.");
                    return;
                    // 열쇠 소모에 실패하면 잠금을 해제하지 않는다
                }

                isLocked = false;
                ShowSequence(unlockedSequence, "열쇠로 잠금이 해제되었다.");
                // 열쇠 사용에 성공하면 잠금 상태를 해제한다
            }

            StartCoroutine(MoveDoorRoutine(isOpen ? 0f : -90f));
            // 현재 문 상태에 따라 열기 또는 닫기 회전을 시작한다

            isOpen = !isOpen;
            // 문 개방 상태를 반전시킨다
        }

        // 문을 부드럽게 회전시키는 코루틴
        private IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true;
            // 문이 이동 중임을 표시한다

            float duration = 1.0f;
            float elapsed = 0f;

            Quaternion startRot = hinge.localRotation;
            Quaternion targetRot = Quaternion.Euler(0f, targetAngle, 0f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                hinge.localRotation = Quaternion.Lerp(startRot, targetRot, elapsed / duration);
                yield return null;
                // 경과 시간에 따라 문 회전을 보간한다
            }

            hinge.localRotation = targetRot;
            isMoving = false;
            // 회전 완료 후 이동 상태를 해제한다
        }

        // 시퀀스를 출력한다 (직접 입력 우선, 없으면 기본 문구 사용)
        private void ShowSequence(string customSequence, string defaultMessage)
        {
            if (sequenceText == null) return;
            // 시퀀스 매니저가 없으면 출력하지 않는다

            if (!string.IsNullOrEmpty(customSequence))
            {
                sequenceText.ShowMessage(customSequence);
                // 인스펙터에 입력된 시퀀스를 출력한다
            }
            else
            {
                sequenceText.ShowMessage(defaultMessage);
                // 기본 메시지를 출력한다
            }
        }

        // 외부 퍼즐에서 도어 상호작용 가능 여부를 설정한다
        public void SetInteractionEnabled(bool enabled)
        {
            interactionEnabled = enabled;
        }

        // 외부 퍼즐에서 문 잠금 상태를 설정한다
        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }

        #endregion


        #region Property

        // Action UI에 표시할 상호작용 문구를 반환한다
        public string GetActionText()
        {
            return isOpen ? "문 닫기" : "문 열기";
        }

        #endregion
    }
}
