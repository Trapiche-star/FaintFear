using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 열쇠가 있어야 열리는 잠금 문
    /// PlayerStatus_DY의 열쇠 보유 여부를 검사하여 문을 열고 닫는다.
    /// </summary>
    public class DoorLock : Interactive, IActionProvider
    {
        #region Variables

        // 문 회전 축
        private Transform hinge;

        // 문이 현재 움직이는 중인지
        private bool isMoving = false;

        // 문이 열려 있는지 여부
        private bool isOpen = false;

        // 시작 시 잠금 여부
        [SerializeField] private bool isLocked = true;

        // 필요한 열쇠 타입
        [SerializeField] private RoomKeyType requiredKey = RoomKeyType.None;

        // 시퀀스 텍스트 출력용
        [SerializeField] private SequenceTextManager sequenceTextManager;

        // 메시지 표시 시간
        [SerializeField] private float messageDuration = 2.0f;

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            // 첫 번째 자식을 문 회전축으로 사용
            hinge = transform.GetChild(0);
        }

        #endregion

        #region Interaction Logic

        // E 키 상호작용 시 호출
        public override void Interaction()
        {
            // 문이 움직이는 중이면 무시
            if (isMoving)
                return;

            // 플레이어 상태 가져오기
            PlayerStatus player = PlayerStatus.Instance;
            if (player == null)
                return;

            // 잠긴 문 처리
            if (isLocked)
            {
                // 열쇠가 없으면 열리지 않음
                if (!player.HasKey(requiredKey))
                {
                    ShowSequenceMessage("문이 단단히 잠겨 있다.");
                    return;
                }

                // 열쇠가 있으면 잠금 해제
                isLocked = false;
                ShowSequenceMessage("열쇠로 잠금이 해제되었다.");
            }

            // 문 열기 / 닫기
            if (!isOpen)
                StartCoroutine(MoveDoorRoutine(-90f));
            else
                StartCoroutine(MoveDoorRoutine(0f));

            // 문 상태 반전
            isOpen = !isOpen;
        }

        #endregion

        #region Door Animation

        // 문 회전 애니메이션
        private IEnumerator MoveDoorRoutine(float targetAngle)
        {
            isMoving = true;

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

            hinge.localRotation = targetRot;
            isMoving = false;
        }

        #endregion

        #region UI & Message

        // 시퀀스 텍스트 출력
        private void ShowSequenceMessage(string message)
        {
            if (sequenceTextManager == null)
                return;

            sequenceTextManager.gameObject.SetActive(true);
            sequenceTextManager.ShowMessage(message);
            StartCoroutine(HideSequenceAfterDelay());
        }

        // 일정 시간 후 시퀀스 텍스트 숨김
        private IEnumerator HideSequenceAfterDelay()
        {
            yield return new WaitForSeconds(messageDuration);

            if (sequenceTextManager != null)
                sequenceTextManager.gameObject.SetActive(false);
        }

        #endregion

        #region Action Provider

        // ActionUI에 표시할 문구 제공
        public string GetActionText()
        {
            // 잠겨 있고 열쇠가 필요한 경우
            if (isLocked && requiredKey != RoomKeyType.None)
                return "문 열기";

            // 열린 상태면 닫기
            if (isOpen)
                return "문 닫기";

            // 기본 문구
            return "문 열기";
        }

        #endregion
    }
}
