using System.Collections.Generic;
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 문서 퍼즐 진행 관리자
    /// 문서 ID(예: 4,5,6) 기반으로 퍼즐 완료 여부를 판정하고 도어에 전달한다.
    /// 다른 씬에서도 동일한 퍼즐 상태를 유지한다.
    /// </summary>
    public class DocumentPuzzleManager : MonoBehaviour
    {
        #region Variables

        public static DocumentPuzzleManager Instance;

        [Header("Puzzle Target (Document IDs)")]
        [SerializeField] private int[] requiredDocumentIds;
        // 퍼즐에 필요한 문서 ID들 (예: 4,5,6)

        [Header("Door")]
        [SerializeField] private DoorDocumentTrigger targetDoor;
        // 퍼즐 완료 시 잠금 해제될 도어

        private readonly HashSet<int> readIdSet = new HashSet<int>();
        // 읽은 문서 ID 기록(중복 방지)

        private bool isCompleted = false;
        // 퍼즐 완료 여부

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            // 만약 [이미 인스턴스가 존재한다면] [중복 매니저를 제거한다]

            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 다른 씬에서도 퍼즐 상태를 유지한다
        }

        #endregion


        #region Property

        public bool IsCompleted => isCompleted;
        // 퍼즐 완료 상태 반환

        #endregion


        #region Custom Method

        // 문서 ID가 "읽혔다"라고 보고될 때 호출
        public void NotifyDocumentRead(int documentId)
        {
            if (isCompleted) return;
            // 만약 [이미 퍼즐이 완료되었다면] [더 이상 처리하지 않는다]

            if (!IsRequiredId(documentId)) return;
            // 만약 [퍼즐 대상 ID가 아니라면] [이 퍼즐과 무관하므로 무시한다]

            if (!readIdSet.Add(documentId)) return;
            // 만약 [이미 처리된 ID라면] [중복 처리하지 않는다]

            if (!IsAllRequiredRead()) return;
            // 만약 [아직 모든 퍼즐 대상 ID를 읽지 않았다면] [완료 처리하지 않는다]

            CompletePuzzle();
            // 모든 조건이 충족되었으므로 퍼즐 완료 처리
        }

        // 퍼즐 대상 ID인지 확인
        private bool IsRequiredId(int documentId)
        {
            if (requiredDocumentIds == null) return false;
            // 만약 [퍼즐 대상 ID 목록이 없다면] [항상 거짓을 반환한다]

            for (int i = 0; i < requiredDocumentIds.Length; i++)
            {
                if (requiredDocumentIds[i] == documentId)
                    return true;
                // 만약 [현재 ID가 퍼즐 대상 목록에 포함되어 있다면] [대상으로 인정한다]
            }

            return false;
            // 끝까지 비교했지만 일치하는 ID가 없으므로 대상이 아니다
        }

        // 모든 퍼즐 대상 ID를 읽었는지 확인
        private bool IsAllRequiredRead()
        {
            if (requiredDocumentIds == null || requiredDocumentIds.Length == 0)
                return false;
            // 만약 [퍼즐 대상 ID가 설정되지 않았다면] [완료될 수 없으므로 거짓을 반환한다]

            for (int i = 0; i < requiredDocumentIds.Length; i++)
            {
                if (!readIdSet.Contains(requiredDocumentIds[i]))
                    return false;
                // 만약 [아직 읽지 않은 대상 ID가 하나라도 있다면] [완료 조건을 만족하지 못한다]
            }

            return true;
            // 모든 퍼즐 대상 ID를 읽었으므로 완료 조건을 만족한다
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
