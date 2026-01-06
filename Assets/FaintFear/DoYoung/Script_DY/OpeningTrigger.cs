using System.Collections;
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 아이템 발견 시 연출을 담당하는 튜토리얼 이벤트
    /// </summary>
    public class OpeningTrigger : TutorialEventBase
    {
        #region Variables

        [Header("텍스트")]
        [SerializeField] private SequenceTextManager sequenceText;

        [Header("카메라 연출")]
        [SerializeField] private Transform lookTarget;
        [SerializeField] private float lookRotateDuration = 1.0f;

        [Header("대사")]
        [TextArea]
        [SerializeField]
        private string itemDialogue = "내 손전등과 호환되는 배터리가 있다.";

        #endregion


        #region Unity Event Method

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            Play(ItemSequence());

            GetComponent<Collider>().enabled = false;
        }

        #endregion


        #region Sequence

        private IEnumerator ItemSequence()
        {
            // 이동 잠금
            if (playerMove != null)
                playerMove.canMove = false;

            // 카메라 연출
            yield return LookAtTarget();

            yield return new WaitForSeconds(1.5f);

            // 텍스트 출력
            if (sequenceText != null)
                sequenceText.ShowMessage(itemDialogue, 2.5f);

            yield return new WaitForSeconds(1.0f);


            // 이동 복구
            if (playerMove != null)
                playerMove.canMove = true;
        }

        private IEnumerator LookAtTarget()
        {
            if (cameraPosition == null || lookTarget == null)
                yield break;

            Quaternion startRot = cameraPosition.rotation;

            Vector3 dir = (lookTarget.position - cameraPosition.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

            float elapsed = 0f;

            while (elapsed < lookRotateDuration)
            {
                // 씬 이동 / 파괴 안전
                if (cameraPosition == null)
                    yield break;

                elapsed += Time.deltaTime;
                float t = elapsed / lookRotateDuration;
                cameraPosition.rotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            cameraPosition.rotation = targetRot;
        }

        #endregion


        #region Tutorial State

        protected override bool IsTutorialCompleted()
        {
            var data = SaveSystem.LoadPreview();
            return data != null && data.tutorialCompleted;
        }

        #endregion
    }
}