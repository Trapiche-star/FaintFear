using UnityEngine;
using System.IO;

namespace FaintFear
{
    public static class SaveSystem
    {
        static string SavePath =>
            Path.Combine(Application.persistentDataPath, "save.json");

        // ===================== SAVE =====================
        public static void SaveGame(string checkpointId = "", bool tutorialCompleted = false, bool saveWorldObjects = true)
        {
            // ⭐ 씬 전환 중 저장 차단 (핵심)
            if (SceneLoadManager.IsSceneTransitioning)
            {
                Debug.LogWarning("[SaveSystem] 씬 전환 중 저장 차단");
                return;
            }

            // ⭐ 기존 저장 파일 로드 (병합을 위해)
            SaveData data = LoadPreview();
            if (data == null)
            {
                Debug.Log("[SaveSystem] 기존 저장 파일 없음 - 새로 생성");
                data = new SaveData();
            }
            else
            {
                Debug.Log("[SaveSystem] 기존 저장 파일 로드 - 데이터 병합 모드");
            }

            // ===================== 플레이어 상태 =====================
            if (PlayerStatus.Instance != null)
            {
                data.mental = PlayerStatus.Instance.currentMentalPower;
                data.battery = PlayerStatus.Instance.currentBattery;
                data.batteryCount = PlayerStatus.Instance.batteryCount;
            }

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                data.playerPosition = player.transform.position;
                data.playerRotation = player.transform.rotation;
            }

            data.savedSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // ===================== 진행 상태 =====================
            data.checkpointId = checkpointId;
            data.tutorialCompleted = tutorialCompleted || data.tutorialCompleted;
            data.lightsPermaOff = data.tutorialCompleted;

            // ===================== ⭐ 월드 오브젝트 상태 저장 (병합) =====================
            if (saveWorldObjects)
            {
                var behaviours = Object.FindObjectsByType<MonoBehaviour>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None
                );

                foreach (var behaviour in behaviours)
                {
                    if (behaviour is ISaveableWorldObject saveable)
                    {
                        saveable.Save(ref data);
                    }
                }

                // ⭐ PlayerStatus도 명시적으로 저장
                if (PlayerStatus.Instance != null)
                {
                    PlayerStatus.Instance.Save(ref data);
                }

                // ⭐ 런타임 상태도 병합
                RuntimeStateManager.MergeRuntimeStateToSaveData(ref data);
            }

            // ===================== 파일 저장 =====================
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"[SaveSystem] ========== 게임 저장 완료 ==========");
            Debug.Log($"[SaveSystem] 저장 경로: {SavePath}");
            Debug.Log($"[SaveSystem] 저장된 씬: {data.savedSceneName}");
            Debug.Log($"[SaveSystem] 파괴된 오브젝트 ({data.destroyedObjects.Count}개): {string.Join(", ", data.destroyedObjects)}");
            Debug.Log($"[SaveSystem] 이동된 오브젝트: {data.movedObjects.Count}개");
            Debug.Log($"[SaveSystem] 문 상태 ({data.doorStates.Count}개): {string.Join(", ", data.doorStates.ConvertAll(d => $"{d.id}(잠금:{d.isLocked})"))}");
            Debug.Log($"[SaveSystem] 읽은 문서: {data.readDocuments.Count}개");
            Debug.Log($"[SaveSystem] 보유 열쇠: {string.Join(", ", data.ownedKeys)}");
            Debug.Log($"[SaveSystem] 보유 레버: [{string.Join(", ", data.ownedLevers)}]");
            Debug.Log($"[SaveSystem] =====================================");
        }

        // ===================== LOAD (미리보기) =====================
        public static SaveData LoadPreview()
        {
            Debug.Log($"[SaveSystem] LoadPreview 시작");
            Debug.Log($"[SaveSystem] 저장 경로: {SavePath}");
            Debug.Log($"[SaveSystem] 파일 존재: {File.Exists(SavePath)}");

            if (!File.Exists(SavePath))
            {
                Debug.LogWarning("[SaveSystem] 저장 파일이 존재하지 않습니다!");
                return null;
            }

            string jsonText = File.ReadAllText(SavePath);
            Debug.Log($"[SaveSystem] 파일 내용 길이: {jsonText.Length}자");
            Debug.Log($"[SaveSystem] 파일 내용 일부:\n{jsonText.Substring(0, Mathf.Min(500, jsonText.Length))}...");

            SaveData data = JsonUtility.FromJson<SaveData>(jsonText);

            if (data == null)
            {
                Debug.LogError("[SaveSystem] JSON 파싱 실패!");
                return null;
            }

            Debug.Log($"[SaveSystem] LoadPreview 완료:");
            Debug.Log($"  - 저장된 씬: {data.savedSceneName}");
            Debug.Log($"  - 파괴된 오브젝트: {data.destroyedObjects.Count}개");
            Debug.Log($"  - 이동된 오브젝트: {data.movedObjects.Count}개");
            Debug.Log($"  - 문 상태: {data.doorStates.Count}개");

            return data;
        }

        // ===================== 월드 오브젝트 LOAD =====================
        public static void ApplyWorldObjectLoad()
        {
            SaveData data = LoadPreview();
            if (data == null)
            {
                Debug.LogWarning("[SaveSystem] 로드할 데이터 없음");
                return;
            }

            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            Debug.Log($"[SaveSystem] ========== 월드 오브젝트 로드 시작 ==========");
            Debug.Log($"[SaveSystem] 현재 씬: {currentScene}");
            Debug.Log($"[SaveSystem] 저장된 씬: {data.savedSceneName}");
            Debug.Log($"[SaveSystem] SaveData에 저장된 상태:");
            Debug.Log($"  - 파괴된 오브젝트 ({data.destroyedObjects.Count}개): {string.Join(", ", data.destroyedObjects)}");
            Debug.Log($"  - 이동된 오브젝트 ({data.movedObjects.Count}개): {string.Join(", ", data.movedObjects.ConvertAll(m => m.id))}");
            Debug.Log($"  - 문 상태 ({data.doorStates.Count}개): {string.Join(", ", data.doorStates.ConvertAll(d => $"{d.id}(잠금:{d.isLocked})"))}");
            Debug.Log($"  - 읽은 문서 ({data.readDocuments.Count}개): {string.Join(", ", data.readDocuments)}");

            var behaviours =
                Object.FindObjectsByType<MonoBehaviour>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None
                );

            Debug.Log($"[SaveSystem] 현재 씬에서 발견된 ISaveableWorldObject: {behaviours.Length}개");

            int loadedCount = 0;
            foreach (var behaviour in behaviours)
            {
                if (behaviour is ISaveableWorldObject saveable)
                {
                    string id = saveable.GetID();
                    saveable.Load(data);
                    loadedCount++;

                    // 상세 로그
                    if (data.destroyedObjects.Contains(id))
                    {
                        Debug.Log($"[SaveSystem] 로드: {id} → 비활성화");
                    }
                    else if (data.doorStates.Exists(d => d.id == id))
                    {
                        var doorData = data.doorStates.Find(d => d.id == id);
                        Debug.Log($"[SaveSystem] 로드: {id} → 문 상태(잠금:{doorData.isLocked})");
                    }
                }
            }

            Debug.Log($"[SaveSystem] 로드 완료 - {loadedCount}개 오브젝트 처리");
            Debug.Log($"[SaveSystem] ==========================================");
        }

        // ===================== UTIL =====================
        public static bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public static void DeleteSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
    }
}