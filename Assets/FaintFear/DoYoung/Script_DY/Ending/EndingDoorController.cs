using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 정문 상호작용 컨트롤러
    /// 엔딩 조건을 확인하거나 실패 시 순차 시퀀스를 출력한다
    /// </summary>
    public class EndingDoorController : Interactive, IActionProvider
    {
        #region Variables

        [Header("Reference")]
        [SerializeField] private AEndingTrigger endingATrigger;    // 엔딩 A 트리거
        [SerializeField] private BEndingTrigger endingBTrigger;    // 엔딩 B 트리거
        [SerializeField] private SequenceTextManager textManager;  // 시퀀스 텍스트 매니저

        [Header("Fail Lines")]
        [SerializeField] private string fail_First;                // 실패 문구 1
        [SerializeField] private string fail_Second;               // 실패 문구 2
        [SerializeField] private string fail_Third;                // 실패 문구 3
        [SerializeField] private float lineHoldTime = 2.5f;        // 문구 유지 시간

        [Header("Action Text")]
        [SerializeField] private string actionText = "문";         // ActionUI 문구

        private int failCount = 0;                                  // 실패 상호작용 카운트
        private bool isLocked = false;                              // 시퀀스 출력 중 잠금

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            if (isLocked)
                return; // 만약 [시퀀스 출력 중이라면] [상호작용을 차단한다]

            if (EndingManager.Instance == null)
                return; // 만약 [엔딩 매니저 인스턴스가 없다면] [처리하지 않는다]

            if (EndingManager.Instance.CanEnterEndingA())
            {
                if (endingATrigger != null)
                    endingATrigger.ExecuteEnding();

                //+ 정문 열리는 사운드 재생
                if (SoundManager.Instance != null)
                    {
                        SoundManager.Instance.PlaySFX("SFX_ExitOpen");
                        Debug.Log("[EndingDoor] 정문 열림 사운드 재생");
                    }

                return; // 만약 [엔딩 A 조건이 충족되었다면] [엔딩 A를 실행한다]
            }

            if (EndingManager.Instance.CanEnterEndingB())
            {
                if (endingBTrigger != null)
                    endingBTrigger.ExecuteEnding();

                //+ 정문 열리는 사운드 재생
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySFX("SFX_ExitOpen");
                    Debug.Log("[EndingDoor] 정문 열림 사운드 재생");
                }

                return; // 만약 [엔딩 B 조건이 충족되었다면] [엔딩 B를 실행한다]
            }

            HandleFailSequence();
            // 조건 미충족 시 실패 시퀀스를 출력한다
        }

        // 실패 시퀀스를 순차적으로 출력한다
        private void HandleFailSequence()
        {
            failCount++;

            int index = failCount % 3;
            // 상호작용 횟수를 3으로 나눈 나머지를 계산한다

            if (index == 1)
                PlayFailSequence(fail_First);
            else if (index == 2)
                PlayFailSequence(fail_Second);
            else
                PlayFailSequence(fail_Third);
        }

        // 단일 실패 시퀀스를 출력한다
        private void PlayFailSequence(string message)
        {
            if (textManager == null)
                return; // 만약 [텍스트 매니저가 없다면] [출력을 중단한다]

            StartCoroutine(PlayAndHide(message));
        }

        // 시퀀스 출력 → 유지 → 종료
        private IEnumerator PlayAndHide(string message)
        {
            isLocked = true;

            yield return StartCoroutine(
                textManager.ShowDialogueSequence(
                    new string[] { message },
                    lineHoldTime
                )
            );

            textManager.Hide();
            isLocked = false;
        }

        #endregion


        #region Property

        public string GetActionText()
        {
            return actionText;
            // 항상 동일한 문구를 반환한다
        }

        #endregion
    }
}
