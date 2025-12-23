using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 볼트 커터를 획득하는 퍼즐용 픽업 오브젝트
    /// </summary>
    public class BoltCutterPickup : Interactive, IActionProvider
    {
        #region Variables

        private HUDManager hud; // HUD 메시지 출력 담당

        #endregion


        #region Unity Event Method

        // 픽업 오브젝트 초기 설정
        private void Awake()
        {
            // 씬에 존재하는 HUDManager를 탐색하여 참조한다
            hud = Object.FindFirstObjectByType<HUDManager>();
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            // 퍼즐 인벤토리가 존재하지 않으면 진행할 수 없으므로 중단한다
            if (PuzzleInventory.Instance == null)
                return;

            // 이미 볼트 커터를 보유 중이라면 중복 획득을 방지한다
            if (PuzzleInventory.Instance.HasBoltCutter)
            {
                ShowHUDMessage("이미 볼트 커터를 가지고 있다.");
                return;
            }

            // 퍼즐 인벤토리에 볼트 커터 보유 상태를 등록한다
            PuzzleInventory.Instance.AcquireBoltCutter();

            // 획득 메시지를 HUD에 출력한다
            ShowHUDMessage("볼트 커터를 획득했다.");

            // 월드 픽업 오브젝트를 제거한다
            gameObject.SetActive(false);
        }

        // HUD 메시지 출력 위임
        private void ShowHUDMessage(string message)
        {
            // HUDManager가 존재할 경우에만 메시지를 출력한다
            if (hud != null)
                hud.ShowDialogue(message);
        }

        #endregion


        #region Property

        // 액션 UI에 표시될 문구 제공
        public string GetActionText()
        {
            return "볼트 커터 줍기";
        }

        #endregion
    }
}
