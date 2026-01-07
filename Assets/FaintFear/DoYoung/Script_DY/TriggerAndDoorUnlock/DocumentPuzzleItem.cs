using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 문서 퍼즐 대상 오브젝트
    /// 해당 문서가 닫힐 때 퍼즐 매니저에 "읽음(ID)"을 보고한다.
    /// </summary>
    public class DocumentPuzzleItem : MonoBehaviour
    {
        #region Variables

        [Header("Document ID")]
        [SerializeField] private int documentId; // 이 문서의 고유 ID (예: 4,5,6)

        private bool isRead = false; // 중복 처리 방지

        #endregion

        #region Custom Method

        // 문서가 "읽힘"으로 판정될 때 호출
        public void MarkAsRead()
        {
            Debug.Log("[PuzzleItem] MarkAsRead: " + gameObject.name + " (ID: " + documentId + ")");

            if (isRead) return;
            // 만약 [이미 읽은 문서라면] [다시 처리하지 않는다]

            isRead = true;
            // 최초 1회만 읽음 상태로 전환한다

            if (DocumentPuzzleManager.Instance != null)
            {
                DocumentPuzzleManager.Instance.NotifyDocumentRead(documentId);
                // 만약 [퍼즐 매니저가 존재한다면] [문서 ID 기반으로 읽음 상태를 보고한다]
            }
            else
            {
                Debug.Log("[PuzzleItem] DocumentPuzzleManager.Instance is NULL");
            }
        }

        #endregion
    }
}
