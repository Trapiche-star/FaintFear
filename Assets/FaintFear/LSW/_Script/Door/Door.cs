using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 문 상호작용 처리
    /// </summary>
    public class Door : Interactive
    {
        #region Variables
        Transform hinge;
        bool isMoving = false; // 문이 움직이는 중인지 확인
        bool isOpen = false;   // 문이 현재 열려있는지 상태 확인 (true: 열림, false: 닫힘)
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            hinge = transform.GetChild(0);
        }
        #endregion

        #region Custom Method

        public override void Interaction()
        {
            // 문이 움직이는 중이라면 입력 무시
            if (isMoving) return;

            if (!isOpen)
            {
                // 닫혀있으면 -> 연다 (목표 각도 -90도)
                Debug.Log("문 여는 중");
                StartCoroutine(MoveDoorRoutine(-90f));
            }
            else
            {
                // 열려있으면 -> 닫는다 (목표 각도 0도)
                Debug.Log("문 닫는 중");
                StartCoroutine(MoveDoorRoutine(0f));
            }

            // 상태 반전 (열림 <-> 닫힘)
            isOpen = !isOpen;
        }

        IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true; // 움직임 시작

            float duration = 1.0f;
            float elapsedTime = 0f;

            // 시작 회전값
            Quaternion startRotation = hinge.localRotation;
            // 목표 회전값 (인자로 받은 targetAngle 사용)
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                // 선형 보간으로 부드럽게 회전
                hinge.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);

                yield return null;
            }

            // 최종 각도 확실하게 고정
            hinge.localRotation = targetRotation;

            isMoving = false; // 움직임 종료
        }

        #endregion
    }

}

