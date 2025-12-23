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

        [SerializeField] private bool isLocked = true;           // 초기 잠금 상태
        [SerializeField] private RoomKeyType requiredKey;        // 문에 필요한 열쇠 타입

        private HUDManager hud;                  // HUD 메시지 출력 담당

        #endregion


        #region Unity Event Method

        // 문 초기 설정 및 HUD 참조 준비
        private void Awake()
        {
            hinge = transform.GetChild(0);       // 첫 번째 자식을 문 힌지로 사용
            hud = Object.FindFirstObjectByType<HUDManager>(); // 씬 내 첫 HUDManager를 일관되게 탐색
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            // 문이 이미 움직이는 중이면 중복 입력 방지
            if (isMoving)
                return;

            PlayerStatus player = PlayerStatus.Instance; // 플레이어 상태 접근
            if (player == null)
                return;

            // 문이 잠겨 있는 경우
            if (isLocked)
            {
                // 열쇠가 없으면 실패 메시지 출력 후 종료
                if (!player.HasKey(requiredKey))
                {
                    ShowHUDMessage("문이 단단히 잠겨 있다.");
                    return;
                }

                // 열쇠가 있으면 잠금 해제 메시지 출력
                isLocked = false;
                ShowHUDMessage("열쇠로 잠금이 해제되었다.");
            }

            // 문 상태에 따라 열기 또는 닫기 실행
            StartCoroutine(MoveDoorRoutine(isOpen ? 0f : -90f));

            isOpen = !isOpen; // 문 상태 반전
        }

        // 문 회전 애니메이션 처리
        private IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true; // 애니메이션 시작 표시

            float duration = 1.0f;               // 회전 소요 시간
            float elapsed = 0f;                  // 경과 시간

            Quaternion startRot = hinge.localRotation;                   // 시작 회전값
            Quaternion targetRot = Quaternion.Euler(0f, targetAngle, 0f); // 목표 회전값

            // 지정 시간 동안 회전 보간 수행
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime; // 프레임 시간 누적
                hinge.localRotation = Quaternion.Lerp(startRot, targetRot, elapsed / duration);
                yield return null;
            }

            hinge.localRotation = targetRot; // 최종 회전값 고정
            isMoving = false;                // 애니메이션 종료
        }

        // HUD에 메시지 출력 위임
        private void ShowHUDMessage(string message)
        {
            if (hud != null)                 // HUDManager가 존재할 경우에만
                hud.ShowDialogue(message);   // 메시지 출력 요청
        }

        // 외부 퍼즐 오브젝트에서 문 잠금 상태를 제어한다
        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }

        #endregion


        #region Property

        // Action UI에 표시될 문구 제공
        public string GetActionText()
        {
            return isOpen ? "문 닫기" : "문 열기"; // 문 상태에 따라 문구 반환
        }

        #endregion
    }
}
