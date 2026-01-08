using System.Collections.Generic;
using UnityEngine;

namespace FaintFear
{
    public class DocumentPuzzleManager : MonoBehaviour
    {
        #region Variables

        public static DocumentPuzzleManager Instance;

        [Header("Puzzle Target (Document IDs)")]
        [SerializeField] private int[] requiredDocumentIds;

        [Header("Door")]
        [SerializeField] private DoorDocumentTrigger targetDoor;

        private readonly HashSet<int> readIdSet = new HashSet<int>();
        private bool isCompleted = false;

        #endregion

        #region Unity Event Method

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ⭐ 추가: 씬 로드 시 저장된 상태 복원
        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            // 씬 로드 시 저장된 문서 읽음 상태를 복원
            RestoreReadDocuments();

            // targetDoor 재연결 (씬마다 새로 찾아야 함)
            if (targetDoor == null)
            {
                targetDoor = FindFirstObjectByType<DoorDocumentTrigger>();
            }

            // 퍼즐이 완료되었다면 문 잠금 해제
            if (isCompleted && targetDoor != null)
            {
                targetDoor.SetUnlocked(true);
            }
        }

        #endregion

        #region Property

        public bool IsCompleted => isCompleted;

        #endregion

        #region Custom Method

        public void NotifyDocumentRead(int documentId)
        {
            if (isCompleted) return;

            if (!IsRequiredId(documentId)) return;

            if (!readIdSet.Add(documentId)) return;

            // ⭐ 런타임 상태에 기록
            RuntimeStateManager.RecordDocumentRead($"Document_{documentId}");

            if (!IsAllRequiredRead()) return;

            CompletePuzzle();
        }

        // ⭐ 추가: 저장된 문서 읽음 상태 복원
        private void RestoreReadDocuments()
        {
            SaveData data = SaveSystem.LoadPreview();
            if (data == null) return;

            // SaveData에서 읽은 문서 복원
            foreach (string docId in data.readDocuments)
            {
                // "Document_5" 형식에서 숫자만 추출
                if (docId.StartsWith("Document_"))
                {
                    string idStr = docId.Substring(9); // "Document_" 이후 문자열
                    if (int.TryParse(idStr, out int id))
                    {
                        if (IsRequiredId(id))
                        {
                            readIdSet.Add(id);
                        }
                    }
                }
            }

            // 모든 문서를 읽었는지 확인
            if (IsAllRequiredRead())
            {
                isCompleted = true;
                Debug.Log("[DocumentPuzzle] 저장된 데이터에서 퍼즐 완료 상태 복원");
            }
        }

        // ⭐ 추가: 외부에서 특정 문서 ID가 이미 읽혔는지 확인
        public bool IsDocumentRead(int documentId)
        {
            return readIdSet.Contains(documentId);
        }

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

        private void CompletePuzzle()
        {
            isCompleted = true;

            if (targetDoor != null)
                targetDoor.SetUnlocked(true);

            // ⭐ 퍼즐 완료 시 자동 저장
            AutoSaveManager.Instance?.RequestSave("document_puzzle_complete");
            Debug.Log("[DocumentPuzzle] 퍼즐 완료 - 자동 저장 요청");
        }

        #endregion
    }
}