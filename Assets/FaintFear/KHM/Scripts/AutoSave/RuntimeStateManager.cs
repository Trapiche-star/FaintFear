using UnityEngine;
using System.Collections.Generic;

namespace FaintFear
{
    /// <summary>
    /// 런타임 중 월드 오브젝트 상태를 메모리에만 유지
    /// 체크포인트에서만 SaveData로 병합
    /// </summary>
    public class RuntimeStateManager : MonoBehaviour
    {
        public static RuntimeStateManager Instance { get; private set; }

        // =====================
        // Runtime-only State
        // =====================

        private static HashSet<string> runtimeDestroyedObjects = new();
        private static Dictionary<string, Vector3> runtimeMovedObjects = new();
        private static Dictionary<string, DoorStateData> runtimeDoorStates = new();
        private static HashSet<string> runtimeReadDocuments = new();

        private static PowerBoxData runtimePowerBoxState = null;
        private static ElevatorData runtimeElevatorState = null;
        private static EndingData runtimeEndingState = null;
        private static Dictionary<string, EnemyRuntimeState> runtimeEnemyStates = new();

        // =====================
        // Unity
        // =====================
        public static void RestoreRuntimeStateFromSaveData(SaveData data)
        {
            ClearRuntimeState();

            if (data == null) return;

            foreach (var id in data.destroyedObjects)
                runtimeDestroyedObjects.Add(id);

            foreach (var m in data.movedObjects)
                runtimeMovedObjects[m.id] = m.position;

            foreach (var d in data.doorStates)
            {
                if (!string.IsNullOrEmpty(d.id))
                    runtimeDoorStates[d.id] = d;
            }

            foreach (var doc in data.readDocuments)
                runtimeReadDocuments.Add(doc);

            runtimePowerBoxState = data.powerBoxData;
            runtimeElevatorState = data.elevatorData;
            runtimeEndingState = data.endingData;

            Debug.Log("[RuntimeState] SaveData → RuntimeState 복원 완료");
        }

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

        // =====================
        // Record (런타임 기록)
        // =====================
        public static void RecordEndingState(bool[] activatedLevers)
        {
            if (runtimeEndingState == null)
                runtimeEndingState = new EndingData();

            runtimeEndingState.activatedLevers = activatedLevers;
        }
        public static void RecordDestroyedObject(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            runtimeDestroyedObjects.Add(id);
        }

        public static void RecordMovedObject(string id, Vector3 position)
        {
            if (string.IsNullOrEmpty(id)) return;
            runtimeMovedObjects[id] = position;
        }

        public static void RecordDoorState(string id, bool isOpen, bool isLocked)
        {
            if (string.IsNullOrEmpty(id)) return;

            if (!runtimeDoorStates.ContainsKey(id))
                runtimeDoorStates[id] = new DoorStateData { id = id };

            runtimeDoorStates[id].isOpen = isOpen;
            runtimeDoorStates[id].isLocked = isLocked;
        }

        // ⭐ 문서 읽음 기록 (복구됨)
        public static void RecordDocumentRead(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            runtimeReadDocuments.Add(id);
        }

        // ⭐ PowerBox 기록
        public static void RecordPowerBoxState(
            string id,
            bool[] filledSlots,
            bool[] leverObjectsActive,
            bool isPowerSupplied,
            bool isCompleted)
        {
            if (runtimePowerBoxState == null)
                runtimePowerBoxState = new PowerBoxData();

            runtimePowerBoxState.filledSlots = filledSlots;
            runtimePowerBoxState.leverObjectsActive = leverObjectsActive;
            runtimePowerBoxState.isPowerSupplied = isPowerSupplied;
            runtimePowerBoxState.isCompleted = isCompleted;
        }

        // ⭐ Elevator 기록
        public static void RecordElevatorState(bool isPowerSupplied)
        {
            if (runtimeElevatorState == null)
                runtimeElevatorState = new ElevatorData();

            runtimeElevatorState.isPowerSupplied = isPowerSupplied;
        }
        public static void RecordEnemyState(string id, EnemyRuntimeState state)
        {
            if (string.IsNullOrEmpty(id)) return;
            runtimeEnemyStates[id] = state;
        }

        // =====================
        // Apply (씬 로드 시)
        // =====================

        public static void ApplyRuntimeState()
        {
            var behaviours = Object.FindObjectsByType<MonoBehaviour>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

            foreach (var behaviour in behaviours)
            {
                if (behaviour is not ISaveableWorldObject saveable)
                    continue;

                string id = saveable.GetID();
                if (string.IsNullOrEmpty(id))
                    continue;

                // 비활성화
                if (runtimeDestroyedObjects.Contains(id))
                    behaviour.gameObject.SetActive(false);

                // 이동
                if (runtimeMovedObjects.TryGetValue(id, out var pos))
                    behaviour.transform.position = pos;

                // 문 상태
                if (runtimeDoorStates.ContainsKey(id))
                {
                    SaveData temp = new SaveData();
                    temp.doorStates.Add(runtimeDoorStates[id]);
                    saveable.Load(temp);
                }

                // 문서 읽음
                if (runtimeReadDocuments.Contains(id))
                {
                    SaveData temp = new SaveData();
                    temp.readDocuments.Add(id);
                    saveable.Load(temp);
                }

                // PowerBox
                if (runtimePowerBoxState != null && behaviour is PowerBoxController)
                {
                    SaveData temp = new SaveData();
                    temp.powerBoxData = runtimePowerBoxState;
                    saveable.Load(temp);
                }

                // Elevator
                if (runtimeElevatorState != null && behaviour is ElevatorManager)
                {
                    SaveData temp = new SaveData();
                    temp.elevatorData = runtimeElevatorState;
                    saveable.Load(temp);
                }

                if (runtimeEndingState != null && behaviour is EndingManager)
                {
                    SaveData temp = new SaveData();
                    temp.endingData = runtimeEndingState;
                    saveable.Load(temp);
                }

                if (behaviour is Enemy_Ex enemy)
                {
                    string enemyId = enemy.GetEnemyId();
                    if (runtimeEnemyStates.TryGetValue(enemyId, out var state))
                    {
                        enemy.RestoreRuntimeState(state);
                    }
                }
            }

            Debug.Log("[RuntimeState] 런타임 상태 적용 완료");
        }

        // =====================
        // Merge → SaveData
        // =====================

        public static void MergeRuntimeStateToSaveData(ref SaveData data)
        {
            // PowerBox
            if (runtimePowerBoxState != null)
                data.powerBoxData = runtimePowerBoxState;

            // Elevator
            if (runtimeElevatorState != null)
                data.elevatorData = runtimeElevatorState;

            // Destroyed
            foreach (var id in runtimeDestroyedObjects)
            {
                if (!data.destroyedObjects.Contains(id))
                    data.destroyedObjects.Add(id);
            }

            // Moved
            foreach (var kvp in runtimeMovedObjects)
            {
                var existing = data.movedObjects.Find(m => m.id == kvp.Key);
                if (existing != null)
                    existing.position = kvp.Value;
                else
                    data.movedObjects.Add(new MovedObjectData
                    {
                        id = kvp.Key,
                        position = kvp.Value
                    });
            }

            // Door
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
                    data.doorStates.Add(kvp.Value);
                }
            }

            // Documents
            foreach (var id in runtimeReadDocuments)
            {
                if (!data.readDocuments.Contains(id))
                    data.readDocuments.Add(id);
            }

            if (runtimeEndingState != null)
                data.endingData = runtimeEndingState;

            Debug.Log("[RuntimeState] 런타임 → SaveData 병합 완료");
        }

        // =====================
        // Clear
        // =====================

        public static void ClearRuntimeState()
        {
            runtimeDestroyedObjects.Clear();
            runtimeMovedObjects.Clear();
            runtimeDoorStates.Clear();
            runtimeReadDocuments.Clear();
            runtimePowerBoxState = null;
            runtimeElevatorState = null;
            runtimeEndingState = null;

            Debug.Log("[RuntimeState] 런타임 상태 초기화");
        }
    }
}