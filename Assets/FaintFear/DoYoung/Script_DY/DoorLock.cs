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

        private Transform hinge;                 // 문 회전을 담당하는 힌지 트랜스폼
        private bool isMoving = false;            // 문 애니메이션 진행 여부
        private bool isOpen = false;              // 문 개방 상태

        [SerializeField]
        private bool interactionEnabled = true;  // 도어 상호작용 활성 여부

        [SerializeField]
        private bool isLocked = true;             // 초기 잠금 상태

        [SerializeField]
        private RoomKeyType requiredKey;          // 문에 필요한 열쇠 타입

        [SerializeField]
        private SequenceTextManager sequenceText; // 텍스트 출력과 시퀀스를 담당

        #endregion


        #region Unity Event Method

        // 문 초기 설정
        private void Awake()
        {
            hinge = transform.GetChild(0);
            // 첫 번째 자식을 문 힌지로 사용한다
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            if (!interactionEnabled) return;
            // 만약 [상호작용이 비활성화 상태라면] [이 메서드에서는 더 이상 처리하지 않는다]

            if (isMoving) return;
            // 만약 [문이 이미 움직이고 있다면] [중복 입력을 방지하기 위해 종료한다]

            PlayerStatus player = PlayerStatus.Instance;
            if (player == null) return;
            // 만약 [플레이어 상태 정보가 없다면] [상호작용을 중단한다]

            // ===================== 잠금 상태 처리 =====================

            if (isLocked)
            {
                if (!player.HasKey(requiredKey))
                {
                    ShowHUDMessage("문이 단단히 잠겨 있다.");
                    return;
                }
                // 만약 [필요한 열쇠가 없다면] [잠김 메시지를 출력하고 종료한다]

                if (!player.ConsumeKey(requiredKey))
                {
                    ShowHUDMessage("열쇠를 사용할 수 없다.");
                    return;
                }
                // 만약 [열쇠 소모에 실패했다면] [잠금 해제를 진행하지 않는다]

                isLocked = false;
                ShowHUDMessage("열쇠로 잠금이 해제되었다.");
                // 열쇠 사용 성공 시 잠금을 해제하고 안내 메시지를 출력한다
            }

            // ===================== 문 열기 / 닫기 =====================

            StartCoroutine(MoveDoorRoutine(isOpen ? 0f : -90f));
            // 문 상태에 따라 목표 회전 각도를 설정하여 회전을 시작한다

            isOpen = !isOpen;
            // 문 상태를 반전시킨다
        }

        // 문 회전 애니메이션 처리
        private IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true;
            // 문 이동 중 상태로 설정한다

            float duration = 1.0f;
            float elapsed = 0f;

            Quaternion startRot = hinge.localRotation;
            Quaternion targetRot = Quaternion.Euler(0f, targetAngle, 0f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                hinge.localRotation = Quaternion.Lerp(startRot, targetRot, elapsed / duration);
                yield return null;
            }
            // 지정된 시간 동안 부드럽게 목표 각도로 회전시킨다

            hinge.localRotation = targetRot;
            isMoving = false;
            // 회전 완료 후 이동 상태를 해제한다
        }

        // HUD 메시지 출력
        private void ShowHUDMessage(string message)
        {
            if (sequenceText == null)
            {
#if UNITY_EDITOR
                if (isLocked)
                    Debug.LogWarning($"{name}: 잠긴 문이지만 SequenceTextManager가 연결되지 않았습니다.");
#endif
                return;
            }
            // 만약 [시퀀스 텍스트가 연결되지 않았다면] [경고 후 메시지 출력을 생략한다]

            sequenceText.ShowMessage(message);
            // HUD 메시지를 출력한다
        }

        // 외부 퍼즐에서 상호작용 가능 여부를 제어
        public void SetInteractionEnabled(bool enabled)
        {
            interactionEnabled = enabled;
        }

        // 외부 퍼즐에서 잠금 상태를 제어
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
