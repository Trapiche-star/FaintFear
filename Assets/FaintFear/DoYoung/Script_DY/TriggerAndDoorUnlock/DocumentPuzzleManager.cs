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

            // targetDoor 재연결
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

            RuntimeStateManager.RecordDocumentRead($"Document_{documentId}");

            if (!IsAllRequiredRead()) return;

            CompletePuzzle();
        }

        // ⭐ 추가: 상태 초기화
        public void ResetState()
        {
            readIdSet.Clear();
            isCompleted = false;

            Debug.Log("[DocumentPuzzleManager] 상태 초기화 완료");
        }

        private void RestoreReadDocuments()
        {
            SaveData data = SaveSystem.LoadPreview();
            if (data == null) return;

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

            AutoSaveManager.Instance?.RequestSave("document_puzzle_complete");
            Debug.Log("[DocumentPuzzle] 퍼즐 완료 - 자동 저장 요청");
        }

        #endregion
    }
}