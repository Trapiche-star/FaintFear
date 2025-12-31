using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 양쪽으로 열리는 더블 도어를 제어하는 상호작용 도어
    /// </summary>
    public class DoubleDoorLock : Interactive, IActionProvider
    {
        #region Variables

        [Header("Door Hinges")]
        [SerializeField] private Transform leftHinge;   // 왼쪽 문 회전을 담당
        [SerializeField] private Transform rightHinge;  // 오른쪽 문 회전을 담당

        [Header("Open Angles")]
        [SerializeField] private float leftOpenAngle = -90f;   // 왼쪽 문 열림 각도
        [SerializeField] private float rightOpenAngle = 90f;   // 오른쪽 문 열림 각도

        [Header("Lock Settings")]
        [SerializeField] private bool isLocked = true;          // 초기 잠금 상태
        [SerializeField] private RoomKeyType requiredKey;       // 문 개방에 필요한 열쇠 타입

        [Header("Sequence")]
        [SerializeField] private SequenceTextManager sequenceText; // HUD 시퀀스 출력 관리자

        [Header("Custom Sequences")]
        [SerializeField, TextArea] private string lockedSequence;        // 열쇠 미보유 시 출력
        [SerializeField, TextArea] private string unlockedSequence;      // 잠금 해제 시 출력
        [SerializeField, TextArea] private string cannotUseKeySequence;  // 열쇠 사용 실패 시 출력

        private bool isMoving = false;   // 문 애니메이션 진행 여부
        private bool isOpen = false;     // 문 개방 상태

        #endregion


        #region Unity Event Method

        // 초기 참조 상태를 점검한다
        private void Awake()
        {
            if (sequenceText == null) return;
            // 만약 [텍스트 매니저가 연결되지 않았다면] [아무 처리도 하지 않는다]
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용을 처리한다
        public override void Interaction()
        {
            if (isMoving) return;
            // 만약 [문이 현재 이동 중이라면] [중복 상호작용을 차단한다]

            PlayerStatus player = PlayerStatus.Instance;
            // 플레이어 상태 싱글톤을 참조한다

            if (player == null) return;
            // 만약 [플레이어 상태가 존재하지 않으면] [처리를 중단한다]

            if (isLocked)
            {
                // 문이 잠겨 있는 상태라면 열쇠 검사를 진행한다

                if (!player.HasKey(requiredKey))
                {
                    ShowSequence(
                        lockedSequence,
                        "문이 단단히 잠겨 있다."
                    );
                    // 만약 [열쇠를 보유하지 않았다면] [잠김 시퀀스를 출력한다]

                    return;
                    // 열쇠가 없으므로 더 이상 진행하지 않는다
                }

                if (!player.ConsumeKey(requiredKey))
                {
                    ShowSequence(
                        cannotUseKeySequence,
                        "열쇠를 사용할 수 없다."
                    );
                    // 만약 [열쇠 소모에 실패했다면] [실패 시퀀스를 출력한다]

                    return;
                    // 열쇠 사용이 실패했으므로 처리를 중단한다
                }

                isLocked = false;
                // 열쇠 사용에 성공했으므로 잠금 상태를 해제한다

                ShowSequence(
                    unlockedSequence,
                    "열쇠로 잠금이 해제되었다."
                );
                // 잠금 해제 시퀀스를 출력한다
            }

            StartCoroutine(MoveDoorRoutine(isOpen));
            // 현재 문 상태를 기준으로 열기 또는 닫기 애니메이션을 시작한다

            isOpen = !isOpen;
            // 문 개방 상태를 반전시킨다
        }

        // 양쪽 문을 동시에 회전시키는 애니메이션을 처리한다
        private IEnumerator MoveDoorRoutine(bool opened)
        {
            isMoving = true;
            // 문이 이동 중임을 표시한다

            float duration = 1.0f;
            // 문 애니메이션에 소요될 시간을 정의한다

            float elapsed = 0f;
            // 경과 시간을 초기화한다

            Quaternion leftStart = leftHinge.localRotation;
            // 왼쪽 문의 시작 회전값을 저장한다

            Quaternion rightStart = rightHinge.localRotation;
            // 오른쪽 문의 시작 회전값을 저장한다

            Quaternion leftTarget =
                Quaternion.Euler(0f, opened ? 0f : leftOpenAngle, 0f);
            // 현재 상태에 따라 왼쪽 문의 목표 회전값을 계산한다

            Quaternion rightTarget =
                Quaternion.Euler(0f, opened ? 0f : rightOpenAngle, 0f);
            // 현재 상태에 따라 오른쪽 문의 목표 회전값을 계산한다

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                // 프레임 경과 시간만큼 누적한다

                leftHinge.localRotation =
                    Quaternion.Lerp(leftStart, leftTarget, elapsed / duration);
                // 왼쪽 문을 목표 회전값으로 보간한다

                rightHinge.localRotation =
                    Quaternion.Lerp(rightStart, rightTarget, elapsed / duration);
                // 오른쪽 문을 목표 회전값으로 보간한다

                yield return null;
                // 다음 프레임까지 대기한다
            }

            leftHinge.localRotation = leftTarget;
            // 왼쪽 문의 최종 회전값을 보정한다

            rightHinge.localRotation = rightTarget;
            // 오른쪽 문의 최종 회전값을 보정한다

            isMoving = false;
            // 문 이동이 완료되었음을 표시한다
        }

        // 시퀀스를 출력한다 (직접 입력 우선, 없으면 기본 문구 사용)
        private void ShowSequence(string customSequence, string defaultMessage)
        {
            if (sequenceText == null) return;
            // 만약 [텍스트 매니저가 존재하지 않으면] [출력을 중단한다]

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

        #endregion


        #region Property

        // Action UI에 표시할 문구를 제공한다
        public string GetActionText()
        {
            return isOpen ? "문 닫기" : "문 열기";
            // 문 상태에 따라 액션 텍스트를 반환한다
        }

        #endregion
    }
}
