using UnityEngine;
using UnityEngine.UI;

namespace FaintFear
{
    /// <summary>
    /// 문서 아이템 상호작용 처리
    /// </summary>
    public class PickupDocument : Interactive, IActionProvider
    {
        #region Variables
        private PlayerMove playerMove;
        
        [SerializeField] private GameObject documentUI;
        [SerializeField] private Sprite documentSprite;

        private Image documentUIImage;
        #endregion

        private void Awake()
        {
            // Player는 씬 유지 대상이므로 안전하게 찾기
            playerMove = FindAnyObjectByType<PlayerMove>();

            // ❌ 여기서 UI 접근하지 않는다 (중요)
        }

        public override void Interaction()
        {
            if (documentUI == null || playerMove == null)
                return;

            // 필요 시점에만 UI 캐싱
            if (documentUIImage == null)
                documentUIImage = documentUI.GetComponentInChildren<Image>(true);

            // 문서 표시
            documentUIImage.sprite = documentSprite;
            documentUI.SetActive(true);

            // 플레이어 제어 잠금
            playerMove.enabled = false;

            // 시스템 일시 정지
            PlayerStatus.Instance.isMentalSystemActive = false;
            PlayerStatus.Instance.isBatteryActive = false;
        }

        /// <summary>
        /// 문서 닫기 (버튼에서 호출)
        /// </summary>
        public void CloseDocument()
        {
            if (documentUI != null)
                documentUI.SetActive(false);

            // 🔑 반드시 원복
            if (playerMove != null)
                playerMove.enabled = true;

            PlayerStatus.Instance.isMentalSystemActive = true;
            PlayerStatus.Instance.isBatteryActive = true;
        }

        public string GetActionText()
        {
            return "문서";
        }
    }
}
