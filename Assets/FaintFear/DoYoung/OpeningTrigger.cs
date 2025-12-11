using System.Collections;
using UnityEngine;
using TMPro;

namespace FaintFear
{
    /// <summary>
    /// 오프닝 연출용 트리거:
    /// - 플레이어가 트리거에 들어오면 조작을 잠그고
    /// - 카메라를 배터리 쪽으로 돌려 보여준 뒤
    /// - 대사 출력 후 다시 조작을 풀어준다
    /// </summary>
    public class OpeningTrigger : MonoBehaviour
    {
        [Header("참조 대상")]
        [SerializeField] private Transform lookTarget;            // 카메라가 바라볼 대상 (예: 배터리)
        [SerializeField] private SceneFader fadeFader;            // SceneFader 스크립트가 붙은 Canvas
        [SerializeField] private TextMeshProUGUI sequenceText;    // 텍스트 (SequenceText)
        [SerializeField] private GameObject sequenceUI;           // UI 패널 (SequenceUI)

        [Header("연출 설정값")]
        [SerializeField] private float lookRotateDuration = 1.0f;  // 카메라 회전 시간
        [SerializeField] private float dialogueHoldTime = 2.0f;    // 대사 유지 시간
        [SerializeField] private float fadeDuration = 1.0f;        // 페이드 인/아웃 시간
        [SerializeField, TextArea] private string dialogueLine = "내 손전등과 호환되는 배터리가 있다."; // 출력할 대사

        private bool hasPlayed = false;

        private void Awake()
        {
            // SceneFader 자동 참조 보완
            if (fadeFader == null)
                fadeFader = FindObjectOfType<SceneFader>();

            // SequenceUI는 시작 시 꺼둠
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
            // 1️⃣ 플레이어 이동 스크립트 잠금
            PlayerMove playerMove = playerCollider.GetComponent<PlayerMove>();
            if (playerMove != null)
                playerMove.enabled = false;

            // 2️⃣ 카메라 찾기
            Transform cameraTransform = null;
            if (playerMove != null)
            {
                Camera cam = playerMove.GetComponentInChildren<Camera>();
                if (cam != null)
                    cameraTransform = cam.transform;
            }

            if (cameraTransform == null)
            {
                Debug.LogWarning("[OpeningTrigger] 카메라를 찾을 수 없습니다.");
                yield break;
            }

            // 3️⃣ 페이드 인 (까맣던 화면을 서서히 밝게)
            if (fadeFader != null)
                fadeFader.FadeOutToZero();

            yield return new WaitForSeconds(fadeDuration);

            // 4️⃣ 카메라 회전 (lookTarget 쪽으로)
            if (lookTarget != null)
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

            // 5️⃣ 텍스트 출력
            if (sequenceUI != null)
                sequenceUI.SetActive(true);

            if (sequenceText != null)
            {
                sequenceText.text = dialogueLine;
                Debug.Log($"[OpeningTrigger] 텍스트 출력됨: {dialogueLine}");
            }
            else
            {
                Debug.LogWarning("[OpeningTrigger] SequenceText가 연결되지 않았습니다!");
            }

            yield return new WaitForSeconds(dialogueHoldTime);

            if (sequenceUI != null)
                sequenceUI.SetActive(false);

            // 6️⃣ 플레이어 다시 조작 가능
            if (playerMove != null)
                playerMove.enabled = true;
        }
    }
}
