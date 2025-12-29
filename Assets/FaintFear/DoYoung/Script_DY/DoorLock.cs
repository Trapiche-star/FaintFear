using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 열쇠 보유 여부를 검사하여 잠긴 문을 열고 닫는 상호작용 도어
    /// </summary>
    public class DoorLock : Interactive, IActionProvider
    {
        #region Variables

        private Transform hinge;                // 문 회전을 담당하는 힌지 트랜스폼
        private bool isMoving = false;           // 문 애니메이션 진행 여부
        private bool isOpen = false;             // 문 개방 상태

        [SerializeField]
        private bool interactionEnabled = true; // 도어락 상호작용 활성 여부

        [SerializeField]
        private bool isLocked = true;            // 초기 잠금 상태

        [SerializeField]
        private RoomKeyType requiredKey;         // 문에 필요한 열쇠 타입

        [SerializeField]
        private SequenceTextManager sequenceText; // HUD 텍스트 출력 담당 (System UI)

        #endregion


        #region Unity Event Method

        // 문 초기 설정
        private void Awake()
        {
            // 첫 번째 자식을 문 힌지로 사용한다
            hinge = transform.GetChild(0);

            // 텍스트 매니저가 지정되지 않았으면 경고만 출력한다
            if (sequenceText == null)
                Debug.LogWarning($"{name}: SequenceTextManager가 지정되지 않음");
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            // 외부 퍼즐에 의해 상호작용이 차단된 경우 입력을 무시한다
            if (!interactionEnabled)
                return;

            // 문이 이미 움직이는 중이면 중복 입력을 방지한다
            if (isMoving)
                return;

            PlayerStatus player = PlayerStatus.Instance;
            if (player == null)
                return;

            // ===================== 잠금 상태 처리 =====================

            // 문이 잠겨 있는 경우
            if (isLocked)
            {
                // 열쇠가 없으면 실패 메시지 출력 후 종료한다
                if (!player.HasKey(requiredKey))
                {
                    ShowHUDMessage("문이 단단히 잠겨 있다.");
                    return;
                }

                // 열쇠 소모에 실패하면 잠금 해제하지 않는다
                if (!player.ConsumeKey(requiredKey))
                {
                    ShowHUDMessage("열쇠를 사용할 수 없다.");
                    return;
                }

                // 열쇠 사용 성공 → 잠금 해제
                isLocked = false;
                ShowHUDMessage("열쇠로 잠금이 해제되었다.");
            }

            // ===================== 문 열기 / 닫기 =====================

            // 문 상태에 따라 회전 목표 각도를 결정한다
            StartCoroutine(MoveDoorRoutine(isOpen ? 0f : -90f));

            // 문 상태를 반전시킨다
            isOpen = !isOpen;
        }

        // 문 회전 애니메이션 처리
        private IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true;

            float duration = 1.0f;
            float elapsed = 0f;

            Quaternion startRot = hinge.localRotation;
            Quaternion targetRot = Quaternion.Euler(0f, targetAngle, 0f);

            // 지정된 시간 동안 부드럽게 회전시킨다
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                hinge.localRotation = Quaternion.Lerp(startRot, targetRot, elapsed / duration);
                yield return null;
            }

            hinge.localRotation = targetRot;
            isMoving = false;
        }

        // HUD 메시지 출력
        private void ShowHUDMessage(string message)
        {
            if (sequenceText == null)
                return;

            sequenceText.ShowMessage(message);
        }

        // 외부 퍼즐에서 상호작용 가능 여부를 제어한다
        public void SetInteractionEnabled(bool enabled)
        {
            interactionEnabled = enabled;
        }

        // 외부 퍼즐에서 잠금 상태를 제어한다
        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }

        #endregion


        #region Property

        // Action UI에 표시될 문구 제공
        public string GetActionText()
        {
            return isOpen ? "문 닫기" : "문 열기";
        }

        #endregion
    }
}
