using UnityEngine;

namespace FaintFear
{
    public class DocumentPuzzleItem : MonoBehaviour
    {
        #region Variables

        [Header("Document ID")]
        [SerializeField] private int documentId;
        private bool isRead = false;

        #endregion

        #region Unity Event Method

        // ⭐ 추가: 시작 시 저장된 상태 복원
        private void Start()
        {
            if (DocumentPuzzleManager.Instance != null)
            {
                // 이미 읽은 문서라면 상태 복원
                if (DocumentPuzzleManager.Instance.IsDocumentRead(documentId))
                {
                    isRead = true;
                    Debug.Log($"[PuzzleItem] 저장된 상태 복원: {gameObject.name} (ID: {documentId}) - 이미 읽음");
                }
            }
        }

        #endregion

        #region Custom Method

        public void MarkAsRead()
        {
            Debug.Log($"[PuzzleItem] MarkAsRead: {gameObject.name} (ID: {documentId})");

            if (isRead) return;

            isRead = true;

            if (DocumentPuzzleManager.Instance != null)
            {
                DocumentPuzzleManager.Instance.NotifyDocumentRead(documentId);
            }
            else
            {
                Debug.LogWarning("[PuzzleItem] DocumentPuzzleManager.Instance is NULL");
            }
        }

        #endregion
    }
}