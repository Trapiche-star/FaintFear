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
            SaveData prev = LoadPreview();
            SaveData data = new SaveData();

            // ===================== 플레이어 상태 =====================
            data.mental = PlayerStatus.Instance.currentMentalPower;
            data.battery = PlayerStatus.Instance.currentBattery;
            data.batteryCount = PlayerStatus.Instance.batteryCount;

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                data.playerPosition = player.transform.position;
                data.playerRotation = player.transform.rotation;
            }

            data.savedSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // ===================== 진행 상태 =====================
            data.checkpointId = checkpointId;
            data.tutorialCompleted = tutorialCompleted || (prev != null && prev.tutorialCompleted);
            data.lightsPermaOff = data.tutorialCompleted;

            // ===================== ⭐ 월드 오브젝트 상태 저장 =====================
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

                // ⭐ PlayerStatus도 명시적으로 저장 (DontDestroyOnLoad라서 FindObjectsByType에 안 잡힐 수 있음)
                if (PlayerStatus.Instance != null)
                {
                    PlayerStatus.Instance.Save(ref data);
                }

                // ⭐ 런타임 상태도 병합
                RuntimeStateManager.MergeRuntimeStateToSaveData(ref data);
            }

            // ===================== 파일 저장 =====================
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"[SaveSystem] 게임 저장 완료 : {SavePath}");
        }

        // ===================== LOAD (미리보기) =====================
        public static SaveData LoadPreview()
        {
            if (!File.Exists(SavePath))
                return null;

            return JsonUtility.FromJson<SaveData>(
                File.ReadAllText(SavePath)
            );
        }

        // ===================== 월드 오브젝트 LOAD =====================
        public static void ApplyWorldObjectLoad()
        {
            SaveData data = LoadPreview();
            if (data == null)
                return;

            var behaviours =
                Object.FindObjectsByType<MonoBehaviour>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None
                );

            foreach (var behaviour in behaviours)
            {
                if (behaviour is ISaveableWorldObject saveable)
                {
                    saveable.Load(data);
                }
            }

            Debug.Log("[SaveSystem] 월드 오브젝트 Load 적용 완료");
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