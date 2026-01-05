using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 시체 상호작용 오브젝트
    /// 상호작용 횟수와 대거 보유 상태에 따라 단일 시퀀스를 출력한다
    /// </summary>
    public class SeveredArm : Interactive, IActionProvider
    {
        #region Variables

        [Header("Reference")]
        [SerializeField] private SequenceTextManager textManager; // 텍스트 출력과 시퀀스를 담당
        private PlayerMove playerMove;                             // 플레이어 이동 제어

        [Header("No Dagger")]        
        [SerializeField] private string noDagger_First;           // 첫 조사
        [SerializeField] private string noDagger_Second;          // 두 번째 조사

        [Header("With Dagger")]
        [SerializeField] private string dagger_First;             // 첫 사용
        [SerializeField] private string dagger_Second;            // 두 번째 사용
        [SerializeField] private string dagger_Acquire;           // 획득 문구

        [Header("After Acquire")]
        [SerializeField] private string acquired_First;           // 이미 획득 1
        [SerializeField] private string acquired_Second;          // 이미 획득 2

        [Header("Timing")]
        [SerializeField] private string dagger_Hold;              // 정적 홀드중 연출 시퀀스 텍스트
        [SerializeField] private float silentHoldTime = 3f;       // 정적 연출 시간
        [SerializeField] private float textHoldTime = 2.5f;       // 텍스트 유지 시간

        private int interactCount = 0;                             // 상호작용 횟수
        private bool isAcquired = false;                           // 잘린 팔 획득 여부
        private bool isLocked = false;                             // 시퀀스 출력 중 잠금 여부
        private bool hasEnteredDaggerState = false;                // 대거 상태 최초 진입 여부

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            playerMove = FindFirstObjectByType<PlayerMove>();
            // 플레이어 이동 컴포넌트를 탐색한다
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            if (isLocked)
                return; // 만약 [시퀀스 출력 중이라면] [상호작용을 차단한다]

            if (PuzzleInventory.Instance == null)
                return; // 만약 [퍼즐 인벤토리가 없다면] [상호작용을 중단한다]

            // 이미 잘린 팔을 획득한 이후
            if (isAcquired)
            {
                HandleAfterAcquire();
                return;
            }

            // 대거가 없는 상태
            if (!PuzzleInventory.Instance.HasBoltCutter)
            {
                hasEnteredDaggerState = false;
                // 대거 상태에 진입한 기록을 초기화한다

                HandleNoDagger();
                return;
            }

            // 여기부터 대거를 가진 상태
            if (!hasEnteredDaggerState)
            {
                interactCount = 0;
                hasEnteredDaggerState = true;
                // 대거 상태에 처음 진입했으므로 카운트를 초기화한다
            }

            HandleWithDagger();
        }

        // 대거 없음 상태 처리
        private void HandleNoDagger()
        {
            interactCount++;
            // 대거 없음 상태에서만 카운트를 증가시킨다

            if (interactCount % 2 == 1)
                PlaySequence(noDagger_First);
            else
                PlaySequence(noDagger_Second);
        }

        // 대거 보유 상태 처리
        private void HandleWithDagger()
        {
            interactCount++;
            // 대거 보유 상태에서만 카운트를 증가시킨다

            if (interactCount == 1)
            {
                PlaySequence(dagger_First);
                return;
            }

            if (interactCount == 2)
            {
                PlaySequence(dagger_Second);
                return;
            }

            StartCoroutine(AcquireProcess());
        }

        // 획득 이후 처리
        private void HandleAfterAcquire()
        {
            interactCount++;
            // 획득 이후 상태에서만 카운트를 증가시킨다

            int index = interactCount % 2;
            // 상호작용 횟수를 2로 나눈 나머지를 계산한다

            if (index == 1)
                PlaySequence(acquired_First);
            else
                PlaySequence(acquired_Second);
        }

        // 잘린 팔 획득 연출
        private IEnumerator AcquireProcess()
        {
            isLocked = true;
            HoldPlayer(true);
            // 플레이어 이동을 잠근다

            PlaySequence(dagger_Hold);
            // 정적에 들어가기 직전 짧은 심리 묘사를 출력한다

            yield return new WaitForSeconds(silentHoldTime);
            // 정적 상태를 유지한다

            HoldPlayer(false);
            // 정적 종료

            PuzzleInventory.Instance.AddLever(0);
            isAcquired = true;
            interactCount = 0;

            PlaySequence(dagger_Acquire);
        }

        // 단일 시퀀스를 출력하고 자동으로 숨긴다
        private void PlaySequence(string message)
        {
            if (textManager == null)
                return; // 만약 [텍스트 매니저가 없다면] [출력을 중단한다]

            StartCoroutine(PlayAndHide(message));
            // 시퀀스를 출력한 뒤 자동으로 숨기는 코루틴을 실행한다
        }

        // 시퀀스 출력 → 유지 → 자동 종료
        private IEnumerator PlayAndHide(string message)
        {
            isLocked = true;
            // 시퀀스 출력 중 상호작용을 차단한다

            yield return StartCoroutine(
                textManager.ShowDialogueSequence(
                    new string[] { message },
                    textHoldTime
                )
            );
            // 지정된 시간 동안 문구를 출력한다

            textManager.Hide();
            // 시퀀스 출력이 끝났으므로 텍스트를 숨긴다

            isLocked = false;
            // 상호작용 잠금을 해제한다
        }

        // 플레이어 이동 잠금 제어
        private void HoldPlayer(bool hold)
        {
            if (playerMove != null)
                playerMove.enabled = !hold;
            // 만약 [플레이어 이동 컴포넌트가 있다면] [활성 상태를 전환한다]
        }

        #endregion


        #region Property

        // ActionUI에 표시할 문구
        public string GetActionText()
        {
            return "시체";
            // 항상 동일한 대상 문구만 반환한다
        }

        #endregion
    }
}
