using UnityEngine;
using UnityEngine.UI;

namespace FaintFear
{
    public class PickupDocument : Interactive, IActionProvider, ISaveableWorldObject
    {
        #region Variables

        public static PickupDocument Current;
        private PlayerMove playerMove;
        private DocumentPuzzleItem puzzleItem;

        [Header("Save")]
        [SerializeField] private string uniqueId;

        [Header("UI")]
        [SerializeField] private GameObject documentUI;
        [SerializeField] private Sprite documentSprite;
        private Image documentUIImage;

        private bool wasRead = false;

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            puzzleItem = GetComponent<DocumentPuzzleItem>();
        }

        #endregion

        #region Custom Method

        public override void Interaction()
        {
            CachePlayer();

            if (documentUI == null || playerMove == null)
                return;

            if (documentUIImage == null)
                documentUIImage = documentUI.GetComponentInChildren<Image>(true);

            documentUIImage.sprite = documentSprite;
            documentUI.SetActive(true);
            Current = this;

            playerMove.enabled = false;
            PlayerStatus.Instance.isMentalSystemActive = false;
            PlayerStatus.Instance.isBatteryActive = false;
        }

        public void CloseDocument()
        {
            CachePlayer();

            if (documentUI != null)
                documentUI.SetActive(false);

            if (playerMove != null)
                playerMove.enabled = true;

            PlayerStatus.Instance.isMentalSystemActive = true;
            PlayerStatus.Instance.isBatteryActive = true;

            if (puzzleItem != null)
                puzzleItem.MarkAsRead();

            if (!wasRead)
            {
                wasRead = true;
                RuntimeStateManager.RecordDocumentRead(uniqueId);
                AutoSaveManager.Instance?.RequestSave($"document_read_{uniqueId}");
            }

            if (Current == this)
                Current = null;
        }

        #endregion

        #region Helper

        private void CachePlayer()
        {
            if (playerMove != null) return;
            playerMove = Object.FindFirstObjectByType<PlayerMove>();
        }

        #endregion

        #region Property

        public string GetActionText() => "[E] 문서";

        #endregion

        #region Save

        public string GetID() => uniqueId;

        public void Save(ref SaveData data)
        {
            if (wasRead && !data.readDocuments.Contains(uniqueId))
                data.readDocuments.Add(uniqueId);
        }

        public void Load(SaveData data)
        {
            wasRead = data.readDocuments.Contains(uniqueId);
        }

        #endregion
    }
}
