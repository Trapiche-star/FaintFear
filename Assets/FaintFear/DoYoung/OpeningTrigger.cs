using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 오프닝 시퀀스 트리거, (오프닝 페이더 추가예정)
    /// </summary>
    public class OpeningTrigger : MonoBehaviour
    {
        [Header("참조 대상")]
        [SerializeField] private Transform lookTarget;         // 카메라가 바라볼 대상 (예: 배터리)
        public TextMeshProUGUI sequenceText; // 대사 출력용 텍스트


        [Header("설정값")]
        [SerializeField] private float lookRotateDuration = 1.0f;  // 카메라 회전 시간
        [SerializeField] private float dialogueHoldTime = 1.2f;    // 대사 유지 시간
        [SerializeField, TextArea] private string dialogueLine = "내 손전등과 호환되는 배터리가 있다.";

        private bool hasPlayed = false;
        public CinemachineCamera vcam;
        public Transform playerCamera;


        private void OnTriggerEnter(Collider other)
        {
            if (hasPlayed) return;
            if (!other.CompareTag("Player")) return;

            hasPlayed = true;

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            StartCoroutine(OpeningSequence(other));
        }

        private IEnumerator OpeningSequence(Collider playerCollider)
        {
            var playerMove = playerCollider.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                // 이동 중일 수 있으니 즉시 입력값 초기화
                playerMove.OnMove(new UnityEngine.InputSystem.InputAction.CallbackContext());
                // 플레이어 조작 비활성화 (정지)
                vcam.enabled = false;
                playerMove.enabled = false;
            }

            // 회전 처리
            if (lookTarget != null && playerCamera != null)
            {
                Quaternion startRot = playerCamera.rotation;
                Vector3 dir = (lookTarget.position - playerCamera.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

                float elapsed = 0f;
                while (elapsed < lookRotateDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / lookRotateDuration);
                    playerCamera.rotation = Quaternion.Slerp(startRot, targetRot, t);
                    yield return null;
                }

                playerCamera.rotation = targetRot;
            }

            // **텍스트 오브젝트 강제 활성화 및 출력**
            if (sequenceText != null)
            {
                sequenceText.gameObject.SetActive(true);
                sequenceText.text = dialogueLine;
            }

            // 대사 유지 시간
            yield return new WaitForSeconds(dialogueHoldTime + 2f); // 🔥 2초 추가 대기

            // **텍스트 비활성화**
            if (sequenceText != null)
                sequenceText.gameObject.SetActive(false);

            // 다시 플레이어 조작 복귀
            if (playerMove != null)
            {
                vcam.enabled = true;
                playerMove.enabled = true;
            }
        }
    }
}
