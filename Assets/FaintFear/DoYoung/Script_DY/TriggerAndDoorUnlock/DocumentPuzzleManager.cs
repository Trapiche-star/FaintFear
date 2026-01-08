using System.Collections.Generic;
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 문서 퍼즐 전체 관리자
    /// 문서 ID 기반으로 읽음 상태를 기록하고 퍼즐 완료 여부를 판정한다
    /// 씬과 무관하게 유지되며 문 오브젝트를 직접 참조하지 않는다
    /// </summary>
    public class DocumentPuzzleManager : MonoBehaviour
    {
        #region Variables

        public static DocumentPuzzleManager Instance;

        [Header("Puzzle Target (Document IDs)")]
        [SerializeField] private int[] requiredDocumentIds; // 퍼즐 완료에 필요한 문서 ID 목록

        private readonly HashSet<int> readIdSet = new HashSet<int>(); // 읽은 문서 ID 집합
        private bool isCompleted = false;                             // 퍼즐 완료 여부

        #endregion


        #region Unity Event Method

        // 싱글톤 구성 및 씬 전환 시 유지 설정
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // 이미 인스턴스가 존재한다면 중복 생성된 자신을 제거한다
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정한다
        }

        // 씬 로드 시 저장된 문서 상태 복원
        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // 씬이 로드되면 저장 데이터 기반으로 읽음 상태를 복원한다
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            RestoreReadDocuments();
        }

        #endregion


        #region Property

        public bool IsCompleted => isCompleted;

        #endregion


        #region Custom Method

        // 문서 읽음 알림을 받아 퍼즐 판정을 갱신한다
        public void NotifyDocumentRead(int documentId)
        {
            if (isCompleted) return;              // 만약 퍼즐이 이미 완료되었다면 더 이상 처리하지 않는다
            if (!IsRequiredId(documentId)) return; // 만약 퍼즐 대상 ID가 아니라면 무시한다
            if (!readIdSet.Add(documentId)) return; // 만약 이미 등록된 ID라면 중복 처리하지 않는다

            RuntimeStateManager.RecordDocumentRead($"Document_{documentId}");

            if (!IsAllRequiredRead()) return;     // 만약 아직 모든 문서가 읽히지 않았다면 완료 처리하지 않는다

            CompletePuzzle();
        }

        // 상태를 초기화한다 (디버그/리셋 용도)
        public void ResetState()
        {
            readIdSet.Clear();
            isCompleted = false;

            Debug.Log("[DocumentPuzzleManager] 상태 초기화 완료");
        }

        // 저장된 데이터에서 읽은 문서 목록을 복원한다
        private void RestoreReadDocuments()
        {
            SaveData data = SaveSystem.LoadPreview();
            if (data == null) return; // 만약 저장 데이터가 없다면 복원하지 않는다

            foreach (string docId in data.readDocuments)
            {
                if (docId.StartsWith("Document_"))
                {
                    string idStr = docId.Substring(9);
                    if (int.TryParse(idStr, out int id))
                    {
                        if (IsRequiredId(id))
                        {
                            readIdSet.Add(id);
                        }
                    }
                }
            }

            if (IsAllRequiredRead())
            {
                isCompleted = true;
                Debug.Log("[DocumentPuzzle] 저장된 데이터에서 퍼즐 완료 상태 복원");
            }
        }

        // 특정 문서가 이미 읽혔는지 확인한다
        public bool IsDocumentRead(int documentId)
        {
            return readIdSet.Contains(documentId);
        }

        // 퍼즐 대상 문서 ID인지 확인한다
        private bool IsRequiredId(int documentId)
        {
            if (requiredDocumentIds == null) return false;

            for (int i = 0; i < requiredDocumentIds.Length; i++)
            {
                if (requiredDocumentIds[i] == documentId)
                    return true;
            }

            return false;
        }

        // 모든 필수 문서가 읽혔는지 확인한다
        private bool IsAllRequiredRead()
        {
            if (requiredDocumentIds == null || requiredDocumentIds.Length == 0)
                return false;

            for (int i = 0; i < requiredDocumentIds.Length; i++)
            {
                if (!readIdSet.Contains(requiredDocumentIds[i]))
                    return false;
            }

            return true;
        }

        // 퍼즐 완료 처리 및 자동 저장을 요청한다
        private void CompletePuzzle()
        {
            isCompleted = true;

            AutoSaveManager.Instance?.RequestSave("document_puzzle_complete");
            Debug.Log("[DocumentPuzzle] 퍼즐 완료 - 자동 저장 요청");
        }

        #endregion
    }
}
