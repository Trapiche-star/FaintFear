using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// UI 활성 상태에서 E 키 입력으로 UI를 닫고 플레이어 제어를 복구한다
    /// </summary>
    public class CloseUIInput : MonoBehaviour
    {
        #region Variables

        [SerializeField] private bool useInteractKey = true; // E 키 사용 여부

        private PlayerMove playerMove;                       // 플레이어 이동 제어
        private PlayerInputAction input;                     // 입력 액션

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            input = new PlayerInputAction();
            // 입력 액션을 초기화한다

            if (useInteractKey)
                input.Player.Interaction.performed += _ => Close();
            // 만약 [E 키 사용이 활성화되어 있다면] [UI 닫기 입력을 등록한다]
        }

        private void OnEnable()
        {
            input.Enable();
            // UI가 활성화될 때 입력을 허용한다

            CachePlayer();
            // 씬 기준으로 플레이어 참조를 다시 확보한다

            if (playerMove != null)
                playerMove.enabled = false;
            // 만약 [플레이어가 존재한다면] [UI 동안 이동을 차단한다]
        }

        private void OnDisable()
        {
            input.Disable();
            // UI가 비활성화될 때 입력을 차단한다
        }

        private void OnDestroy()
        {
            if (useInteractKey)
                input.Player.Interaction.performed -= _ => Close();
            // 오브젝트 파괴 시 입력 이벤트를 해제한다
        }

        #endregion


        #region Custom Method

        // 현재 씬 기준으로 PlayerMove 참조를 다시 확보한다
        private void CachePlayer()
        {
            if (playerMove != null)
                return; // 이미 [플레이어 참조가 있다면] [다시 찾지 않는다]

            playerMove = Object.FindFirstObjectByType<PlayerMove>();
            // Unity 최신 권장 API로 플레이어 이동 컴포넌트를 탐색한다
        }

        // UI를 닫고 플레이어 제어를 복구한다
        private void Close()
        {
            // 만약 [현재 열려 있는 문서가 있다면] [해당 문서의 닫기 처리를 실행한다]
            PickupDocument.Current?.CloseDocument();            

            gameObject.SetActive(false);
            // UI 오브젝트를 비활성화한다

            if (playerMove != null)
                playerMove.enabled = true;
            // 만약 [플레이어 이동 컴포넌트가 존재한다면] [이동을 다시 허용한다]

            if (PlayerStatus.Instance != null)
            {
                PlayerStatus.Instance.isMentalSystemActive = true;
                // 정신력 시스템을 다시 활성화한다

                PlayerStatus.Instance.isBatteryActive = true;
                // 배터리 시스템을 다시 활성화한다
            }
        }

        #endregion
    }
}
