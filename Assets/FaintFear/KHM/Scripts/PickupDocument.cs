using UnityEngine;
using UnityEngine.UI;

namespace FaintFear
{
    public class PickupDocument : Interactive, IActionProvider, ISaveableWorldObject
    {
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

        private void Awake()
        {
            puzzleItem = GetComponent<DocumentPuzzleItem>();
        }

        public override void Interaction()
        {
            CachePlayer();

            if (documentUI == null || playerMove == null)
                return;

            SoundManager.Instance?.PlaySFX("SFX_Paper");

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

            SoundManager.Instance?.PlaySFX("SFX_Paper");

            documentUI?.SetActive(false);

            if (playerMove != null)
                playerMove.enabled = true;

            PlayerStatus.Instance.isMentalSystemActive = true;
            PlayerStatus.Instance.isBatteryActive = true;

            puzzleItem?.MarkAsRead();

            if (!wasRead)
            {
                wasRead = true;
                RuntimeStateManager.RecordDocumentRead(uniqueId);
                AutoSaveManager.Instance?.RequestSave($"document_read_{uniqueId}");
            }

            if (Current == this)
                Current = null;
        }

        private void CachePlayer()
        {
            if (playerMove != null) return;
            playerMove = Object.FindFirstObjectByType<PlayerMove>();
        }

        public string GetActionText() => "[E] 문서";

        // ================= SAVE =================

        public string GetID() => uniqueId;

        public void Save(ref SaveData data)
        {
            if (data.readDocuments == null)
                data.readDocuments = new System.Collections.Generic.List<string>();

            if (wasRead && !data.readDocuments.Contains(uniqueId))
                data.readDocuments.Add(uniqueId);
        }

        public void Load(SaveData data)
        {
            if (data.readDocuments == null)
                return;

            wasRead = data.readDocuments.Contains(uniqueId);
        }
    }
}
