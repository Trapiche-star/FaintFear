using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

namespace FaintFear
{
    /// <summary>
    /// 아이템 발견 시 연출을 담당하는 트리거
    /// </summary>
    public class OpeningTrigger : MonoBehaviour
    {
        #region Variables

        [Header("참조")]
        [SerializeField] private SequenceTextManager sequenceText; // HUD 텍스트 출력 도구
        [SerializeField] private Transform lookTarget;  // 카메라가 바라볼 대상

        [Header("카메라")]
        public CinemachineCamera vcam;                  // 시네머신 가상 카메라
        public Transform playerCamera;                  // 플레이어 실제 카메라

        [Header("연출 설정")]
        [SerializeField] private float lookRotateDuration = 1.0f;
        [SerializeField] private float dialogueHoldTime = 2.5f;

        [Header("아이템 대사")]
        [TextArea]
        [SerializeField]
        private string[] itemDialogueLines =
        {
            "내 손전등과 호환되는 배터리가 있다."
        };

        private bool hasPlayed = false; // 1회 실행 보장

        #endregion


        #region Unity Event Method

        // 플레이어가 트리거에 들어왔을 때 아이템 연출 실행
        private void OnTriggerEnter(Collider other)
        {
            // 만약 이미 실행됐다면 종료
            if (hasPlayed) return;

            // 만약 플레이어가 아닐 때는 무시
            if (!other.CompareTag("Player")) return;

            hasPlayed = true;

            // 그래서 트리거를 다시 못 밟게 막는다
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            StartCoroutine(ItemSequence(other));
        }

        #endregion


        #region Custom Method

        // 아이템 발견 연출 흐름을 처리하는 코루틴
        private IEnumerator ItemSequence(Collider playerCollider)
        {
            var playerMove = playerCollider.GetComponent<PlayerMove>();

            // 만약 플레이어 조작 컴포넌트가 있다면
            if (playerMove != null)
            {
                // 이동 입력을 초기화하고
                playerMove.OnMove(new UnityEngine.InputSystem.InputAction.CallbackContext());

                // 그래서 조작을 잠근다
                playerMove.enabled = false;

                // 그리고 시네머신도 끈다
                if (vcam != null) vcam.enabled = false;
            }

            // 만약 타겟과 카메라가 있을 때만 시점 연출
            if (lookTarget != null && playerCamera != null)
            {
                Quaternion startRot = playerCamera.rotation;
                Vector3 dir = (lookTarget.position - playerCamera.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

                float elapsed = 0f;

                // 그동안 회전 시간이 끝날 때까지 반복
                while (elapsed < lookRotateDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / lookRotateDuration);
                    playerCamera.rotation = Quaternion.Slerp(startRot, targetRot, t);
                    yield return null;
                }

                playerCamera.rotation = targetRot;
            }

            // 아이템 대사 출력
            if (sequenceText != null)
                yield return StartCoroutine(
                    sequenceText.ShowDialogueSequence(itemDialogueLines, dialogueHoldTime)
                );

            // 조작 복구
            if (playerMove != null)
            {
                if (vcam != null) vcam.enabled = true;
                playerMove.enabled = true;
            }
        }

        #endregion
    }
}
