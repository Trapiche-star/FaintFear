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
        private PlayerMove playerMove;                    // 플레이어 이동 제어 컴포넌트

        [Header("카메라")]
        public CinemachineCamera vcam;                  // 시네머신 가상 카메라
        [SerializeField] private Transform cameraPosition;                  // 플레이어 실제 카메라

        [Header("연출 설정")]
        [SerializeField] private float lookRotateDuration = 1.0f;
        [SerializeField] private float dialogueHoldTime = 2.5f;

        [Header("아이템 대사")]
        [TextArea]
        [SerializeField]
        private string itemDialogue ="내 손전등과 호환되는 배터리가 있다.";

        private bool hasPlayed = false; // 1회 실행 보장

        #endregion


        #region Unity Event Method
        private void Awake()
        {
            if (GameManager.TutorialCompleted)
            {
                Destroy(gameObject);
                return;
            }
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerMove = player.GetComponent<PlayerMove>();
            }
        }

        // 플레이어가 트리거에 들어왔을 때 아이템 연출 실행
        private void OnTriggerEnter(Collider other)
        {
            if (IsTutorialCompleted()) return;
            if (hasPlayed) return;
            if (!other.CompareTag("Player")) return;

            cameraPosition = other.transform.Find("CameraPosition");
            if (cameraPosition == null)
            {
                Debug.LogError("[OpeningTrigger] CameraRoot not found");
                return;
            }

            hasPlayed = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(ItemSequence(other));
        }

        #endregion


        #region Custom Method

        // 아이템 발견 연출 흐름을 처리하는 코루틴
        private IEnumerator ItemSequence(Collider playerCollider)
        {
            // 만약 플레이어 조작 컴포넌트가 있다면
            if (playerMove != null)
            {
                // 그래서 조작을 잠근다
                playerMove.canMove = false;
            }

            // 만약 타겟과 카메라가 있을 때만 시점 연출
            if (lookTarget != null && cameraPosition != null)
            {
                Quaternion startRot = cameraPosition.rotation;
                Vector3 dir = (lookTarget.position - cameraPosition.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

                float elapsed = 0f;
                while (elapsed < lookRotateDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / lookRotateDuration;
                    cameraPosition.rotation = Quaternion.Slerp(startRot, targetRot, t);
                    yield return null;
                }
                cameraPosition.rotation = targetRot;
            }
            
            // 아이템 대사 출력
            if (sequenceText != null)
            {
                sequenceText.ShowMessage(itemDialogue, dialogueHoldTime);
            }

            yield return new WaitForSeconds(1.0f);
            // 조작 복구
            if (playerMove != null)
            {
                playerMove.canMove = true;
            }
        }
        private bool IsTutorialCompleted()
        {
            var data = SaveSystem.LoadPreview();
            return data != null && data.tutorialCompleted;
        }
        #endregion
    }
}
