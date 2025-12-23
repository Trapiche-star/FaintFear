using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 볼트 커터 보유 여부에 따라 사슬과 자물쇠를 제거하여 문 봉인을 해제하는 퍼즐 락
    /// </summary>
    public class ChainLock : Interactive, IActionProvider
    {
        #region Variables

        [SerializeField] private GameObject cutRoot; // 절단 시 제거될 체인·자물쇠 오브젝트 묶음
        [SerializeField] private DoorLock doorLock;  // 봉인 해제 대상 도어락

        private SequenceTextManager sequenceText;    // HUD 텍스트 출력 담당
        private bool isUnlocked = false;             // 이미 체인이 제거되었는지 여부

        #endregion


        #region Unity Event Method

        // 체인락 초기 설정 및 HUD 참조 준비
        private void Awake()
        {
            // 씬에 존재하는 SequenceTextManager를 탐색하여 참조한다
            sequenceText = Object.FindFirstObjectByType<SequenceTextManager>();
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            // 이미 체인이 제거된 상태라면 더 이상 반응하지 않는다
            if (isUnlocked)
                return;

            // 퍼즐 인벤토리가 존재하지 않으면 조건 판단이 불가능하므로 중단한다
            if (PuzzleInventory.Instance == null)
                return;

            // 볼트 커터를 보유하지 않은 경우 실패 메시지를 출력한다
            if (!PuzzleInventory.Instance.HasBoltCutter)
            {
                ShowHUDMessage("문이 사슬과 자물쇠로 단단히 감겨 있다.");
                return;
            }

            // 볼트 커터를 보유 중이므로 체인 제거 처리로 넘어간다
            UnlockChain();
        }

        // 사슬과 자물쇠를 제거하고 도어 봉인을 해제한다
        private void UnlockChain()
        {
            // 성공 메시지를 HUD에 출력한다
            ShowHUDMessage("볼트 커터로 자물쇠와 체인을 끊어냈다.");

            // 절단 대상 오브젝트가 존재할 경우 비활성화한다
            if (cutRoot != null)
                cutRoot.SetActive(false);

            // 도어락이 존재할 경우 잠금 상태를 해제한다
            if (doorLock != null)
                doorLock.SetLocked(false);

            // 체인이 제거되었음을 상태로 기록한다
            isUnlocked = true;

            // 체인락 자체를 비활성화하여 문 인터랙션을 허용한다
            gameObject.SetActive(false);
        }

        // SequenceTextManager를 통해 메시지 출력
        private void ShowHUDMessage(string message)
        {
            if (sequenceText != null)
                sequenceText.ShowMessage(message);
        }

        #endregion


        #region Property

        // 액션 UI에 표시될 문구 제공
        public string GetActionText()
        {
            // 이미 해제된 상태라면 문구를 표시하지 않는다
            if (isUnlocked)
                return string.Empty;

            // 볼트 커터 보유 여부에 따라 액션 문구를 분기한다
            if (PuzzleInventory.Instance != null &&
                PuzzleInventory.Instance.HasBoltCutter)
                return "볼트 커터로 자르기";

            return "사슬 조사";
        }

        #endregion
    }
}
