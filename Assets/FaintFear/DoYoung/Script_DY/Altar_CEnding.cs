using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 제단 상호작용 오브젝트
    /// 잘린 팔 보유 여부에 따라 실패 시퀀스를 반복 출력하거나 C 엔딩 씬으로 이동한다
    /// </summary>
    public class Altar_CEnding : Interactive, IActionProvider
    {
        #region Variables

        [Header("Reference")]
        [SerializeField] private SequenceTextManager textManager; // 텍스트 출력과 시퀀스를 담당

        [Header("No Arm Lines")]
        [SerializeField] private string noArm_First;              // 바칠 것이 없음 1
        [SerializeField] private string noArm_Second;             // 바칠 것이 없음 2
        [SerializeField] private string noArm_Third;              // 바칠 것이 없음 3
        [SerializeField] private float lineHoldTime = 2.5f;       // 문구 유지 시간

        [Header("C Ending")]
        [SerializeField] private string cEndingSceneName;         // C 엔딩 씬 이름 (Inspector 지정)

        private int noArmCount = 0;                                // 잘린 팔 미보유 전용 카운트
        private bool isLocked = false;                             // 시퀀스 출력 중 잠금 여부
        private bool isActivated = false;                          // 엔딩 실행 여부

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            if (isLocked)
                return; // 만약 [시퀀스 출력 중이라면] [상호작용을 차단한다]

            if (PuzzleInventory.Instance == null)
                return; // 만약 [퍼즐 인벤토리가 없다면] [상호작용을 중단한다]

            // 잘린 팔이 없는 상태
            if (!PuzzleInventory.Instance.HasAnyLever())
            {
                HandleNoArm();
                return;
            }

            // 이미 엔딩이 실행된 상태
            if (isActivated)
                return; // 만약 [이미 엔딩을 실행했다면] [중복 실행을 차단한다]

            ExecuteCEnding();
            // C 엔딩 씬으로 이동한다
        }

        // 잘린 팔이 없는 상태 처리 (반복)
        private void HandleNoArm()
        {
            noArmCount++;
            // 잘린 팔 미보유 상태에서만 카운트를 증가시킨다

            int index = noArmCount % 3;
            // 상호작용 횟수를 3으로 나눈 나머지를 계산한다

            if (index == 1)
                PlaySequence(noArm_First);
            else if (index == 2)
                PlaySequence(noArm_Second);
            else
                PlaySequence(noArm_Third);
        }

        // C 엔딩을 실행한다
        private void ExecuteCEnding()
        {
            if (string.IsNullOrEmpty(cEndingSceneName))
                return; // 만약 [C 엔딩 씬 이름이 지정되지 않았다면] [이동하지 않는다]

            isActivated = true;
            isLocked = true;
            // 엔딩 실행 중 중복 상호작용을 차단한다

            SceneManager.LoadScene(cEndingSceneName);
            // C 엔딩 씬으로 이동한다
        }

        // 단일 실패 시퀀스를 출력한다
        private void PlaySequence(string message)
        {
            if (textManager == null)
                return; // 만약 [텍스트 매니저가 없다면] [출력을 중단한다]

            StartCoroutine(PlayAndHide(message));
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
            return "제단";
            // 항상 동일한 대상 문구만 반환한다
        }

        #endregion
    }
}
