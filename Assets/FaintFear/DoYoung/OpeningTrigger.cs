using System.Collections;
using UnityEngine;
using TMPro;

namespace FaintFear
{
    /// <summary>
    /// 오프닝 시퀀스 트리거, (오프닝 페이더 추가예정)
    /// </summary>
    public class OpeningTrigger : MonoBehaviour
    {
        [Header("참조 대상")]
        [SerializeField] private Transform lookTarget;         // 카메라가 바라볼 대상 (예: 배터리)
        [SerializeField] private TextMeshProUGUI sequenceText; // 대사 출력용 텍스트
        [SerializeField] private GameObject sequenceUI;         // 대사 UI 전체 패널

        [Header("설정값")]
        [SerializeField] private float lookRotateDuration = 1.0f;  // 카메라 회전 시간
        [SerializeField] private float dialogueHoldTime = 1.2f;    // 대사 유지 시간
        [SerializeField, TextArea] private string dialogueLine = "내 손전등과 호환되는 배터리가 있다.";

        private bool hasPlayed = false;

        private void Awake()
        {
            if (sequenceUI != null)
                sequenceUI.SetActive(false);
        }

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
                playerMove.enabled = false;
            }

            // 카메라 가져오기
            Transform cameraTransform = null;
            if (playerMove != null)
            {
                Camera cam = playerMove.GetComponentInChildren<Camera>();
                if (cam != null)
                    cameraTransform = cam.transform;
            }

            // 회전 처리
            if (lookTarget != null && cameraTransform != null)
            {
                Quaternion startRot = cameraTransform.rotation;
                Vector3 dir = (lookTarget.position - cameraTransform.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

                float elapsed = 0f;
                while (elapsed < lookRotateDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / lookRotateDuration);
                    cameraTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                    yield return null;
                }

                cameraTransform.rotation = targetRot;
            }

            // 회전이 끝난 후 텍스트 출력
            if (sequenceUI != null) sequenceUI.SetActive(true);
            if (sequenceText != null)
                sequenceText.text = dialogueLine;

            yield return new WaitForSeconds(dialogueHoldTime);

            if (sequenceUI != null) sequenceUI.SetActive(false);

            // 다시 플레이어 조작 복귀
            if (playerMove != null)
                playerMove.enabled = true;
        }
    }
}
