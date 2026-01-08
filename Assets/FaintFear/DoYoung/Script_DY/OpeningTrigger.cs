using System.Collections;
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 아이템 발견 시 연출을 담당하는 튜토리얼 이벤트
    /// 텍스트 출력 시간 동안 카메라 시선을 고정한다
    /// </summary>
    public class OpeningTrigger : TutorialEventBase
    {
        #region Variables

        [Header("텍스트")]
        [SerializeField] private SequenceTextManager sequenceText; // 텍스트 출력 담당

        [Header("카메라 연출")]
        [SerializeField] private Transform lookTarget;             // 카메라가 바라볼 대상
        [SerializeField] private float lookRotateDuration = 1.0f; // 시선 회전 시간

        [Header("대사")]
        [TextArea]
        [SerializeField]
        private string itemDialogue = "내 손전등과 호환되는 배터리가 있다."; // 아이템 발견 대사

        #endregion


        #region Unity Event Method

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return; // 플레이어가 아니라면 무시

            Play(ItemSequence());

            GetComponent<Collider>().enabled = false; // 재발동 방지
        }

        #endregion


        #region Sequence

        // 아이템 연출 전체 시퀀스
        private IEnumerator ItemSequence()
        {
            // 이동 잠금
            if (playerMove != null)
                playerMove.canMove = false; // 플레이어 이동 차단

            // 카메라를 대상 방향으로 회전
            yield return LookAtTarget();

            // 텍스트 출력
            if (sequenceText != null)
                sequenceText.ShowMessage(itemDialogue, 2.5f);

            // ⭐ 텍스트 출력 시간 동안 시선 고정
            yield return HoldLookAt(2.5f);

            // 이동 복구
            if (playerMove != null)
                playerMove.canMove = true; // 플레이어 이동 복구
        }

        // 카메라를 부드럽게 대상 방향으로 회전
        private IEnumerator LookAtTarget()
        {
            if (cameraPosition == null || lookTarget == null)
                yield break; // 카메라 또는 타겟이 없다면 중단

            Quaternion startRot = cameraPosition.rotation;

            Vector3 dir = (lookTarget.position - cameraPosition.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

            float elapsed = 0f;

            while (elapsed < lookRotateDuration)
            {
                // 씬 이동 / 파괴 안전
                if (cameraPosition == null)
                    yield break; // 카메라가 파괴되었다면 중단

                elapsed += Time.deltaTime;
                float t = elapsed / lookRotateDuration;
                cameraPosition.rotation = Quaternion.Slerp(startRot, targetRot, t); // 현재 → 목표 보간
                yield return null;
            }

            cameraPosition.rotation = targetRot; // 최종 각도 고정
        }

        // 텍스트 출력 시간 동안 카메라 시선을 강제로 유지
        private IEnumerator HoldLookAt(float holdTime)
        {
            if (cameraPosition == null || lookTarget == null)
                yield break; // 카메라 또는 타겟이 없다면 중단

            Vector3 dir = (lookTarget.position - cameraPosition.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

            float elapsed = 0f;

            while (elapsed < holdTime)
            {
                // 씬 이동 / 파괴 안전
                if (cameraPosition == null)
                    yield break; // 카메라가 파괴되었다면 중단

                cameraPosition.rotation = targetRot; // 매 프레임 목표 방향 유지

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        #endregion


        #region Tutorial State

        protected override bool IsTutorialCompleted()
        {
            var data = SaveSystem.LoadPreview();
            return data != null && data.tutorialCompleted; // 이미 튜토리얼이 끝났다면 실행하지 않음
        }

        #endregion
    }
}
