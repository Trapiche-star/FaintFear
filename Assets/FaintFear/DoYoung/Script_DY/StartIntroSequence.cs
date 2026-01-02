using System.Collections;
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 게임 시작 시 페이드 연출과 오프닝 독백을 실행하고 플레이어 조작을 잠근다
    /// </summary>
    public class StartIntroSequence : MonoBehaviour
    {
        #region Variables
        [Header("참조")]
        [SerializeField] private HUDManager hudManager;
        [SerializeField] private SequenceTextManager sequenceText;

        [Header("연출 설정")]
        [SerializeField] private float dialogueHoldTime = 2f;

        [Header("오프닝 독백")]
        [TextArea]
        [SerializeField]
        private string[] openingDialogueLines =
        {
            "폐병원을 순찰하다 침입자들을 쫓아 들어왔는데… 배터리가 꺼져버렸군.",
        };

        private PlayerMove playerMove;
        #endregion

        #region Unity Event Method

        // ⭐ Awake에서 즉시 플레이어 조작 차단
        private void Awake()
        {
            // 튜토리얼 완료 체크
            var data = SaveSystem.LoadPreview();
            bool tutorialDone = data != null && data.tutorialCompleted;

            if (tutorialDone || GameManager.TutorialCompleted)
            {
                Debug.Log("[StartIntro] Tutorial already done - destroying");
                Destroy(gameObject);
                return;
            }

            // ⭐ 플레이어 즉시 찾아서 조작 차단
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerMove = player.GetComponent<PlayerMove>();
                if (playerMove != null)
                {
                    // ⭐ 이동과 시점 모두 차단
                    playerMove.canMove = false;
                    playerMove.SetLookLock(true);

                    Debug.Log("[StartIntro] Player movement and look locked in Awake");
                }
            }
        }

        private IEnumerator Start()
        {
            // ⭐ 이미 Awake에서 체크했지만 안전을 위해 한 번 더
            var data = SaveSystem.LoadPreview();
            bool tutorialDone = data != null && data.tutorialCompleted;

            if (tutorialDone || GameManager.TutorialCompleted)
            {
                Debug.Log("[StartIntro] Tutorial already done in Start");
                Destroy(gameObject);
                yield break;
            }

            if (hudManager == null || sequenceText == null)
            {
                Debug.LogError("[StartIntro] HUDManager or SequenceText is null!");
                Destroy(this);
                yield break;
            }

            // ⭐ 화면이 검은 상태에서 시작
            yield return new WaitForSeconds(0.3f);

            // ⭐ 페이드 인 (검정 → 밝게)
            Debug.Log("[StartIntro] Starting fade from black");
            hudManager.FadeFromBlack();

            yield return new WaitForSeconds(1f);

            // ⭐ 대사 출력
            yield return StartCoroutine(
                sequenceText.ShowDialogueSequence(
                    openingDialogueLines,
                    dialogueHoldTime
                )
            );

            // ⭐ 텍스트 정리
            sequenceText.Hide();

            // ⭐ 플레이어 조작 복구
            if (playerMove != null)
            {
                playerMove.canMove = true;
                playerMove.SetLookLock(false);
                Debug.Log("[StartIntro] Player controls restored");
            }

            // 1회용
            Destroy(gameObject);
        }
        #endregion
    }
}