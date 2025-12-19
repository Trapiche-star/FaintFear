using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// PowerBox 문 컨트롤러
    /// 문 회전을 담당하고 문 상태에 따라 슬롯 트리거 활성 여부를 제어한다.
    /// </summary>
    public class PB_DoorController : MonoBehaviour
    {
        #region Variables

        // 실제 회전이 일어나는 문 피벗
        [SerializeField]
        private Transform doorPivot;

        // 문이 열릴 때 회전 각도
        [SerializeField]
        private float openAngle = 90f;

        // 문 회전 속도
        [SerializeField]
        private float rotateSpeed = 180f;

        // 파워박스 퍼즐 관리자
        [SerializeField]
        private PowerBoxController powerBox;

        // 문이 열려있는지 상태
        private bool isOpen = false;

        // 문이 회전 중인지 상태
        private bool isMoving = false;

        #endregion


        #region Properties

        // 외부에서 문 상태 확인용
        public bool IsOpen => isOpen;

        #endregion


        #region Public Methods

        // 문 열기/닫기 토글 (DoorTrigger에서 호출)
        public void ToggleDoor()
        {
            // 문이 회전 중이면 입력 무시
            if (isMoving) return;

            // 문 회전 코루틴 시작
            StartCoroutine(RotateDoor());
        }

        #endregion


        #region Private Methods

        // 문을 부드럽게 회전시키는 코루틴
        private IEnumerator RotateDoor()
        {
            // 회전 시작 상태 설정
            isMoving = true;

            // 현재 Y축 회전값
            float startAngle = doorPivot.localEulerAngles.y;

            // 목표 회전값 계산
            float targetAngle = isOpen ? 0f : openAngle;

            // 슬롯 트리거는 문이 완전히 열린 후에만 활성화
            if (!isOpen && powerBox != null)
                powerBox.SetSlotTriggerActive(false);

            // 보간용 시간 변수
            float t = 0f;

            // 회전 보간 처리
            while (t < 1f)
            {
                // 회전 속도에 따른 시간 증가
                t += Time.deltaTime * (rotateSpeed / openAngle);

                // Y축 회전값 보간
                float angle = Mathf.LerpAngle(startAngle, targetAngle, t);

                // 문 피벗 회전 적용
                doorPivot.localEulerAngles = new Vector3(0f, angle, 0f);

                yield return null;
            }

            // 최종 회전값 보정
            doorPivot.localEulerAngles = new Vector3(0f, targetAngle, 0f);

            // 문 열림 상태 반전
            isOpen = !isOpen;

            // 문이 열렸을 때 슬롯 트리거 활성화
            if (isOpen && powerBox != null)
                powerBox.SetSlotTriggerActive(true);

            // 회전 종료 상태 설정
            isMoving = false;
        }

        #endregion
    }
}
