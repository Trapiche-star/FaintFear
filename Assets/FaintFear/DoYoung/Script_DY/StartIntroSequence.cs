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

        // 게임이 시작되자마자 오프닝 연출을 실행한다
        private IEnumerator Start()
        {
            // 만약 HUD 매니저가 없다면 더 이상 진행할 수 없으므로 끝낸다
            if (hudManager == null)
                yield break;

            // 그래서 플레이어를 찾아 조작 컴포넌트를 준비한다
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerMove = player.GetComponent<PlayerMove>();

            // 만약 플레이어 조작 컴포넌트가 존재한다면
            if (playerMove != null)
            {
                // 이동 중 입력이 남아 있을 수 있으므로 입력을 초기화한다
                playerMove.OnMove(new UnityEngine.InputSystem.InputAction.CallbackContext());

                // 그래서 오프닝이 끝날 때까지 조작을 잠근다
                playerMove.enabled = false;
            }            

            // 그래서 페이드 연출 여유를 준다
            yield return new WaitForSeconds(0.5f);

            // 화면을 서서히 밝힌다
            hudManager.FadeFromBlack();

            // 그래서 밝아지는 연출이 보이도록 잠시 기다린다
            yield return new WaitForSeconds(0.5f);

            // 오프닝 독백을 순서대로 출력한다
            yield return StartCoroutine(
                sequenceText.ShowDialogueSequence(
                    openingDialogueLines,
                    dialogueHoldTime
                )
            );

            // 만약 플레이어 조작 컴포넌트가 존재한다면
            if (playerMove != null)
            {
                // 그래서 오프닝이 끝났으므로 조작을 다시 허용한다
                playerMove.enabled = true;
            }
        }

        #endregion
    }
}
