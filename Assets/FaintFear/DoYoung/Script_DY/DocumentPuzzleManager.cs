using System.Collections.Generic;
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 문서 퍼즐 진행 관리자
    /// 지정된 문서 퍼즐 아이템만 인정하여 완료 시 도어 트리거에 전달한다.
    /// </summary>
    public class DocumentPuzzleManager : MonoBehaviour
    {
        #region Variables

        [Header("Puzzle Target")]
        [SerializeField] private DocumentPuzzleItem[] requiredItems;
        // 퍼즐에 필요한 문서 퍼즐 아이템들 (문서 4,5,6)

        [Header("Door")]
        [SerializeField] private DoorDocumentTrigger targetDoor;
        // 퍼즐 완료 시 잠금 해제될 도어

        private readonly HashSet<DocumentPuzzleItem> readSet = new HashSet<DocumentPuzzleItem>();
        // 읽은 문서 퍼즐 아이템 기록(중복 방지)

        private bool isCompleted = false;
        // 퍼즐 완료 여부

        #endregion


        #region Property

        public bool IsCompleted => isCompleted;
        // 퍼즐 완료 상태 반환

        #endregion


        #region Custom Method

        // 문서 퍼즐 아이템이 "읽혔다"라고 보고할 때 호출
        public void NotifyDocumentRead(DocumentPuzzleItem item)
        {
            if (isCompleted) return;
            // 만약 [이미 퍼즐이 완료되었다면] [더 이상 처리하지 않는다]

            if (item == null) return;
            // 만약 [전달된 아이템이 없다면] [처리하지 않는다]

            if (!IsRequiredItem(item)) return;
            // 만약 [퍼즐 대상 아이템이 아니라면] [이 퍼즐과 무관하므로 무시한다]

            if (!readSet.Add(item)) return;
            // 만약 [이미 처리된 아이템이라면] [중복 처리하지 않는다]

            if (!IsAllRequiredRead()) return;
            // 만약 [아직 모든 퍼즐 대상 아이템을 읽지 않았다면] [완료 처리하지 않는다]

            CompletePuzzle();
            // 모든 조건이 충족되었으므로 퍼즐 완료 처리
        }

        // 퍼즐 대상 아이템인지 확인
        private bool IsRequiredItem(DocumentPuzzleItem item)
        {
            if (requiredItems == null) return false;
            // 만약 [퍼즐 대상 아이템 목록이 없다면] [항상 거짓을 반환한다]

            for (int i = 0; i < requiredItems.Length; i++)
            // 퍼즐 대상 아이템 목록을 처음부터 끝까지 순회한다
            {
                if (requiredItems[i] == item)
                    return true;
                // 만약 [현재 아이템이 퍼즐 대상 목록에 포함되어 있다면] [대상 아이템으로 인정한다]
            }

            return false;
            // 끝까지 비교했지만 일치하는 아이템이 없으므로 대상 아이템이 아니다
        }

        // 모든 퍼즐 대상 아이템을 읽었는지 확인
        private bool IsAllRequiredRead()
        {
            if (requiredItems == null || requiredItems.Length == 0)
                return false;
            // 만약 [퍼즐 대상 아이템이 설정되지 않았다면] [완료될 수 없으므로 거짓을 반환한다]

            for (int i = 0; i < requiredItems.Length; i++)
            // 퍼즐에 필요한 모든 아이템을 하나씩 검사한다
            {
                if (!readSet.Contains(requiredItems[i]))
                    return false;
                // 만약 [아직 읽지 않은 대상 아이템이 하나라도 있다면] [완료 조건을 만족하지 못한다]
            }

            return true;
            // 모든 퍼즐 대상 아이템을 읽었으므로 완료 조건을 만족한다
        }

        // 퍼즐 완료 처리
        private void CompletePuzzle()
        {
            isCompleted = true;
            // 퍼즐을 완료 상태로 전환한다

            if (targetDoor != null)
                targetDoor.SetUnlocked(true);
            // 만약 [연결된 도어가 있다면] [잠금 해제를 전달한다]
        }

        #endregion
    }
}
