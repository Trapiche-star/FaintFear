using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 문서 퍼즐 대상 오브젝트
    /// 해당 문서가 닫힐 때 퍼즐 매니저에 "읽음"을 보고한다.
    /// </summary>
    public class DocumentPuzzleItem : MonoBehaviour
    {
        #region Variables

        [SerializeField] private DocumentPuzzleManager puzzleManager; // 퍼즐 매니저
        private bool isRead = false; // 중복 처리 방지

        #endregion

        #region Custom Method

        // 문서가 "읽힘"으로 판정될 때 호출
        public void MarkAsRead()
        {
            Debug.Log("[PuzzleItem] MarkAsRead: " + gameObject.name);

            if (isRead) return;
            // 만약 [이미 읽은 문서라면] [다시 처리하지 않는다]

            isRead = true;
            // 최초 1회만 읽음 상태로 전환한다

            if (puzzleManager != null)
            {
                Debug.Log("[PuzzleItem] Read: " + gameObject.name);
                puzzleManager.NotifyDocumentRead(this);
            }
            else
            {
                Debug.Log("[PuzzleItem] PuzzleManager is NULL on " + gameObject.name);
            }
        }

        #endregion
    }
}
