using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 문 상호작용 처리
    /// </summary>
    public class Door : Interactive, IActionProvider
    {
        #region Variables
        Transform hinge;
        bool isMoving = false; // 문이 움직이는 중인지 확인
        bool isOpen = false;   // 문이 현재 열려있는지 상태 확인
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
            // 문이 움직이는 중이면 무시
            if (isMoving) return;

            if (!isOpen)
            {
                // 🔊 문 열리는 소리
                SoundManager.Instance.PlaySFX("SFX_DoorOpen");

                // 닫혀있으면 -> 연다
                StartCoroutine(MoveDoorRoutine(-90f));
            }
            else
            {
                // 🔊 문 닫히는 소리
                SoundManager.Instance.PlaySFX("SFX_DoorClose");

                // 열려있으면 -> 닫는다
                StartCoroutine(MoveDoorRoutine(0f));
            }

            // 상태 반전
            isOpen = !isOpen;
        }

        IEnumerator MoveDoorRoutine(float targetAngle)
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

        // Action UI에 표시될 문구
        public string GetActionText()
        {
            return isOpen ? "닫기" : "열기";
        }
        #endregion
    }
}
