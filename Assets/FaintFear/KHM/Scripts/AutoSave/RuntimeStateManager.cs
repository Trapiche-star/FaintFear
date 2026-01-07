using UnityEngine;
using System.Collections.Generic;

namespace FaintFear
{
    /// <summary>
    /// 런타임 중 월드 오브젝트 상태를 메모리에만 유지
    /// 체크포인트에서만 파일로 저장
    /// </summary>
    public class RuntimeStateManager : MonoBehaviour
    {
        public static RuntimeStateManager Instance { get; private set; }

        // ⭐ 런타임 중에만 유지되는 임시 상태
        private static HashSet<string> runtimeDestroyedObjects = new HashSet<string>();
        private static Dictionary<string, Vector3> runtimeMovedObjects = new Dictionary<string, Vector3>();
        private static Dictionary<string, DoorStateData> runtimeDoorStates = new Dictionary<string, DoorStateData>();
        private static HashSet<string> runtimeReadDocuments = new HashSet<string>();

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

        // ===================== 런타임 상태 기록 =====================

        public static void RecordDestroyedObject(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[RuntimeState] ID가 null인 오브젝트는 기록할 수 없습니다");
                return;
            }

            runtimeDestroyedObjects.Add(id);
            Debug.Log($"[RuntimeState] 오브젝트 비활성화 기록: {id}");
        }

        public static void RecordMovedObject(string id, Vector3 position)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[RuntimeState] ID가 null인 오브젝트는 기록할 수 없습니다");
                return;
            }

            runtimeMovedObjects[id] = position;
        }

        // ⭐ 문 상태 기록
        public static void RecordDoorState(string id, bool isOpen, bool isLocked)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[RuntimeState] ID가 null인 문은 기록할 수 없습니다");
                return;
            }

            if (!runtimeDoorStates.ContainsKey(id))
            {
                runtimeDoorStates[id] = new DoorStateData { id = id };
            }

            runtimeDoorStates[id].isOpen = isOpen;
            runtimeDoorStates[id].isLocked = isLocked;
            Debug.Log($"[RuntimeState] 문 상태 기록: {id} (Open: {isOpen}, Locked: {isLocked})");
        }

        // ⭐ 문서 읽음 기록
        public static void RecordDocumentRead(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[RuntimeState] ID가 null인 문서는 기록할 수 없습니다");
                return;
            }

            runtimeReadDocuments.Add(id);
            Debug.Log($"[RuntimeState] 문서 읽음 기록: {id}");
        }

        // ===================== 런타임 상태 적용 (씬 로드 시) =====================

        public static void ApplyRuntimeState()
        {
            var behaviours = Object.FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            foreach (var behaviour in behaviours)
            {
                if (behaviour is ISaveableWorldObject saveable)
                {
                    string id = saveable.GetID();

                    // null 체크
                    if (string.IsNullOrEmpty(id))
                    {
                        Debug.LogWarning($"[RuntimeState] ID가 없는 오브젝트 발견: {behaviour.gameObject.name}");
                        continue;
                    }

                    // 런타임에 비활성화된 오브젝트 처리
                    if (runtimeDestroyedObjects.Contains(id))
                    {
                        behaviour.gameObject.SetActive(false);
                        Debug.Log($"[RuntimeState] 런타임 상태 적용 - 비활성화: {id}");
                    }

                    // 런타임에 이동된 오브젝트 처리
                    if (runtimeMovedObjects.ContainsKey(id))
                    {
                        behaviour.transform.position = runtimeMovedObjects[id];
                    }

                    // ⭐ 런타임 문 상태 적용
                    if (runtimeDoorStates.ContainsKey(id))
                    {
                        var doorState = runtimeDoorStates[id];
                        SaveData tempData = new SaveData();
                        tempData.doorStates.Add(doorState);
                        saveable.Load(tempData);
                        Debug.Log($"[RuntimeState] 문 상태 적용: {id}");
                    }

                    // ⭐ 런타임 문서 읽음 상태 적용
                    if (runtimeReadDocuments.Contains(id))
                    {
                        SaveData tempData = new SaveData();
                        tempData.readDocuments.Add(id);
                        saveable.Load(tempData);
                        Debug.Log($"[RuntimeState] 문서 읽음 상태 적용: {id}");
                    }
                }
            }

            Debug.Log($"[RuntimeState] 런타임 상태 적용 완료 - 비활성화: {runtimeDestroyedObjects.Count}, 문: {runtimeDoorStates.Count}, 문서: {runtimeReadDocuments.Count}");
        }

        // ===================== 체크포인트 저장 시 런타임 → SaveData =====================

        public static void MergeRuntimeStateToSaveData(ref SaveData data)
        {
            // 기존 저장된 상태 + 런타임 상태 병합
            foreach (var id in runtimeDestroyedObjects)
            {
                if (!data.destroyedObjects.Contains(id))
                {
                    data.destroyedObjects.Add(id);
                }
            }

            foreach (var kvp in runtimeMovedObjects)
            {
                var existing = data.movedObjects.Find(m => m.id == kvp.Key);
                if (existing != null)
                {
                    existing.position = kvp.Value;
                }
                else
                {
                    data.movedObjects.Add(new MovedObjectData
                    {
                        id = kvp.Key,
                        position = kvp.Value
                    });
                }
            }

            // ⭐ 문 상태 병합
            foreach (var kvp in runtimeDoorStates)
            {
                var existing = data.doorStates.Find(d => d.id == kvp.Key);
                if (existing != null)
                {
                    existing.isOpen = kvp.Value.isOpen;
                    existing.isLocked = kvp.Value.isLocked;
                }
                else
                {
                    data.doorStates.Add(new DoorStateData
                    {
                        id = kvp.Key,
                        isOpen = kvp.Value.isOpen,
                        isLocked = kvp.Value.isLocked
                    });
                }
            }

            // ⭐ 문서 읽음 상태 병합
            foreach (var id in runtimeReadDocuments)
            {
                if (!data.readDocuments.Contains(id))
                {
                    data.readDocuments.Add(id);
                }
            }

            Debug.Log($"[RuntimeState] 런타임 상태 → SaveData 병합 완료");
        }

        // ===================== 새 게임 or 이어하기 시 런타임 상태 초기화 =====================

        public static void ClearRuntimeState()
        {
            runtimeDestroyedObjects.Clear();
            runtimeMovedObjects.Clear();
            runtimeDoorStates.Clear();
            runtimeReadDocuments.Clear();
            Debug.Log("[RuntimeState] 런타임 상태 초기화");
        }
    }
}