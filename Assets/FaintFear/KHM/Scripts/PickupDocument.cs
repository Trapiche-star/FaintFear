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

        // 현재 열려 있는 문서를 전역으로 관리한다
        public static PickupDocument Current;

        private PlayerMove playerMove;

        // 퍼즐 대상 문서(4,5,6)에만 붙어 있는 컴포넌트
        private DocumentPuzzleItem puzzleItem;

        [Header("UI")]
        [SerializeField] private GameObject documentUI; // 문서 UI 루트
        [SerializeField] private Sprite documentSprite; // 표시할 문서 이미지

        private Image documentUIImage;

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            // Player는 씬 유지 대상이므로 안전하게 탐색한다
            playerMove = FindAnyObjectByType<PlayerMove>();

            // 퍼즐 대상 문서에만 컴포넌트가 존재한다
            puzzleItem = GetComponent<DocumentPuzzleItem>();
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리 (문서 열기)
        public override void Interaction()
        {
            if (documentUI == null || playerMove == null)
                return; // 만약 [UI 또는 플레이어 참조가 없다면] [상호작용을 중단한다]

            if (documentUIImage == null)
                documentUIImage = documentUI.GetComponentInChildren<Image>(true);
            // 필요 시점에만 UI 컴포넌트를 캐싱한다

            documentUIImage.sprite = documentSprite;
            // 문서 이미지를 설정한다

            documentUI.SetActive(true);
            // 문서 UI를 표시한다

            // 현재 열린 문서를 기록한다
            Current = this;

            playerMove.enabled = false;
            // 플레이어 이동을 잠근다

            PlayerStatus.Instance.isMentalSystemActive = false;
            PlayerStatus.Instance.isBatteryActive = false;
            // 시스템을 일시 정지한다
        }

        
        // 문서 닫기 (버튼에서 호출)        
        public void CloseDocument()
        {
            //임시 디버그용
            Debug.Log("[PickupDocument] CloseDocument: " + gameObject.name); 

            if (documentUI != null)
                documentUI.SetActive(false);
            // 문서 UI를 숨긴다

            if (playerMove != null)
                playerMove.enabled = true;
            // 플레이어 이동을 복구한다

            PlayerStatus.Instance.isMentalSystemActive = true;
            PlayerStatus.Instance.isBatteryActive = true;
            // 시스템을 원복한다

            if (puzzleItem != null)
                puzzleItem.MarkAsRead();
            // 만약 [퍼즐 대상 문서라면] [퍼즐 진행을 보고한다]

            if (Current == this)
                Current = null;
            // 현재 문서 참조를 해제한다
        }

        #endregion


        #region Property

        // ActionUI에 표시할 문구
        public string GetActionText()
        {
            return "문서";
            // ActionUI에서 자동으로 [E]가 붙어 표시된다
        }
        #endregion        
    }
}
