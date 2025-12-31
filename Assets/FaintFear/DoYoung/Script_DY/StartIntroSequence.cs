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
        [SerializeField] private HUDManager hudManager; // 페이드 효과
        [SerializeField] private SequenceTextManager sequenceText; //텍스트 출력 도구

        [Header("연출 설정")]
        [SerializeField] private float dialogueHoldTime = 2f; // 문장 유지 시간

        [Header("오프닝 독백")]
        [TextArea]
        [SerializeField]
        private string[] openingDialogueLines =
        {
            "폐병원을 순찰하다 침입자들을 쫓아 들어왔는데… 배터리가 꺼져버렸군.",
            
        };

        private PlayerMove playerMove; // 플레이어 조작 컴포넌트

        #endregion


        #region Unity Event Method
        private IEnumerator Start()
        {
            //튜토리얼 이미 끝났으면 즉시 제거
            if (GameManager.TutorialCompleted)
            {
                Destroy(this);
                yield break;
            }

            if (hudManager == null || sequenceText == null)
            {
                Destroy(this);
                yield break;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerMove = player.GetComponent<PlayerMove>();

            if (playerMove != null)
            {
                playerMove.OnMove(new UnityEngine.InputSystem.InputAction.CallbackContext());
                playerMove.enabled = false;
            }

            yield return new WaitForSeconds(0.5f);

            hudManager.FadeFromBlack();
            yield return new WaitForSeconds(0.5f);

            yield return StartCoroutine(
                sequenceText.ShowDialogueSequence(
                    openingDialogueLines,
                    dialogueHoldTime
                )
            );

            //텍스트 정리
            sequenceText.Hide();

            if (playerMove != null)
                playerMove.enabled = true;

            //1회용
            Destroy(this);
        }
        #endregion
    }
}
