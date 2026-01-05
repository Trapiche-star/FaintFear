using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 엘리베이터 상호작용 오브젝트
    /// 전력 상태에 따라 사용 가능 여부를 판단하고 시퀀스를 출력한다
    /// </summary>
    public class ElevatorOpen : Interactive, IActionProvider
    {
        #region Variables

        [Header("Reference")]
        [SerializeField] private ElevatorManager elevatorManager; // 엘리베이터 전력 매니저
        [SerializeField] private SequenceTextManager textManager; // 텍스트 출력 담당

        [Header("Fail Lines")]
        [SerializeField] private string fail_First;               // 전력 미공급 문구 1
        [SerializeField] private string fail_Second;              // 전력 미공급 문구 2
        [SerializeField] private float lineHoldTime = 2.5f;       // 문구 유지 시간

        [Header("Action Text")]
        [SerializeField] private string actionText = "사용하기";  // ActionUI 문구

        private int failCount = 0;                                 // 실패 상호작용 카운트
        private bool isLocked = false;                             // 시퀀스 출력 중 잠금

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            if (isLocked)
                return; // 만약 [시퀀스 출력 중이라면] [상호작용을 차단한다]

            if (elevatorManager == null)
                return; // 만약 [엘리베이터 매니저가 없다면] [처리하지 않는다]

            if (!elevatorManager.IsElevatorAvailable())
            {
                HandleFailSequence();
                return; // 만약 [전력이 공급되지 않았다면] [실패 시퀀스를 출력한다]
            }

            ExecuteElevator();
            // 전력이 공급된 상태이므로 엘리베이터를 실행한다
        }

        // 조건 미충족 시 실패 시퀀스를 출력한다
        private void HandleFailSequence()
        {
            failCount++;
            // 실패 상태에서만 카운트를 증가시킨다

            int index = failCount % 2;
            // 상호작용 횟수를 2로 나눈 나머지를 계산한다

            if (index == 1)
                PlayFailSequence(fail_First);
            else
                PlayFailSequence(fail_Second);
        }

        // 실패 시 단일 시퀀스를 출력한다
        private void PlayFailSequence(string message)
        {
            if (textManager == null)
                return; // 만약 [텍스트 매니저가 없다면] [출력을 중단한다]

            StartCoroutine(PlayAndHide(message));
            // 시퀀스를 출력한 뒤 자동으로 숨긴다
        }

        // 엘리베이터를 실행한다
        private void ExecuteElevator()
        {
            // 여기서 씬 이동 / 연출 / 애니메이션을 처리하면 된다
            Debug.Log("엘리베이터 실행");
        }

        // 시퀀스 출력 → 유지 → 자동 종료
        private IEnumerator PlayAndHide(string message)
        {
            isLocked = true;
            // 시퀀스 출력 중 상호작용을 차단한다

            yield return StartCoroutine(
                textManager.ShowDialogueSequence(
                    new string[] { message },
                    lineHoldTime
                )
            );
            // 지정된 시간 동안 문구를 출력한다

            textManager.Hide();
            // 시퀀스 종료 후 텍스트를 숨긴다

            isLocked = false;
            // 상호작용 잠금을 해제한다
        }

        #endregion


        #region Property

        // ActionUI에 표시할 문구
        public string GetActionText()
        {
            return actionText;
            // 항상 동일한 문구만 반환한다
        }

        #endregion
    }
}
