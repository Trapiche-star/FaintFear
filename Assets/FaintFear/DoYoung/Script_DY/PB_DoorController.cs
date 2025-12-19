using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// PowerBox 문 컨트롤러
    /// 문 회전을 담당하고 문 상태에 따라 슬롯 트리거 활성 여부를 제어한다.
    /// </summary>
    public class PB_DoorController : Interactive, IActionProvider
    {
        #region Variables

        // 실제로 회전하는 문 피벗 (경첩 기준 Transform)
        [SerializeField]
        private Transform doorPivot;

        // 문이 완전히 열려있는 상태의 각도 (모델 기준: 0)
        [SerializeField]
        private float openedAngle = 0f;

        // 문이 완전히 닫혀있는 상태의 각도 (예: 90 또는 -90)
        [SerializeField]
        private float closedAngle = 90f;

        // 문이 열리고 닫히는 속도 (초당 회전 각도)
        [SerializeField]
        private float rotateSpeed = 90f;

        // 파워박스 퍼즐 전체를 관리하는 컨트롤러
        [SerializeField]
        private PowerBoxController powerBox;

        // 현재 문이 열려있는지 여부 (논리 상태)
        private bool isOpen = false;

        // 문이 회전 중인지 여부 (중복 입력 방지용)
        private bool isMoving = false;

        #endregion       


        #region Property

        // 외부에서 문이 열려있는지 확인할 때 사용하는 읽기 전용 프로퍼티
        public bool IsOpen => isOpen;

        #endregion


        #region Custom Method

        // PlayerInteraction에서 E 키 입력 시 호출되는 함수
        public override void Interaction()
        {
            // 문 열기 / 닫기 동작을 토글 방식으로 실행
            ToggleDoor();
        }

        // ActionUI에 표시할 상호작용 문구를 제공
        public string GetActionText()
        {
            // 문이 회전 중이면 UI 문구를 숨김
            if (isMoving)
                return string.Empty;

            // 문 상태에 따라 열기 / 닫기 문구를 다르게 반환
            return isOpen ? "닫기" : "열기";
        }

        // 문 열기 / 닫기를 전환하는 진입 함수
        public void ToggleDoor()
        {
            // 이미 회전 중이면 추가 입력 무시
            if (isMoving) return;

            // 문 회전 처리 코루틴 실행
            StartCoroutine(RotateDoor());
        }

        // 문을 부드럽게 회전시키는 실제 처리 코루틴
        private IEnumerator RotateDoor()
        {
            // 회전 시작 → 입력 잠금
            isMoving = true;

            // 현재 상태에 따라 시작 각도 결정
            // 열려있으면 열린 각도에서 시작
            // 닫혀있으면 닫힌 각도에서 시작
            float startAngle = isOpen ? openedAngle : closedAngle;

            // 목표 각도 결정
            // 열려있으면 닫힌 각도로
            // 닫혀있으면 열린 각도로 이동
            float targetAngle = isOpen ? closedAngle : openedAngle;

            // 문을 닫기 시작할 때 슬롯 트리거 비활성화
            if (isOpen && powerBox != null)
                powerBox.SetSlotTriggerActive(false);

            // 전체 회전에 필요한 시간 계산 (각도 차이 / 속도)
            float duration = Mathf.Abs(targetAngle - startAngle) / rotateSpeed;

            // 경과 시간 누적 변수
            float t = 0f;

            // 회전이 끝날 때까지 반복
            while (t < duration)
            {
                // 프레임 시간만큼 경과 시간 증가
                t += Time.deltaTime;

                // 시작 각도에서 목표 각도까지 선형 보간
                float angle = Mathf.Lerp(startAngle, targetAngle, t / duration);

                // 계산된 각도를 문 피벗에 적용
                doorPivot.localEulerAngles = new Vector3(0f, angle, 0f);

                // 다음 프레임까지 대기
                yield return null;
            }

            // 마지막 프레임 오차 보정을 위해 최종 각도 강제 적용
            doorPivot.localEulerAngles = new Vector3(0f, targetAngle, 0f);

            // 문 상태를 반전 (열림 ↔ 닫힘)
            isOpen = !isOpen;

            // 문이 완전히 열렸다면 슬롯 트리거 다시 활성화
            if (isOpen && powerBox != null)
                powerBox.SetSlotTriggerActive(true);

            // 회전 종료 → 입력 잠금 해제
            isMoving = false;
        }

        #endregion
    }
}
