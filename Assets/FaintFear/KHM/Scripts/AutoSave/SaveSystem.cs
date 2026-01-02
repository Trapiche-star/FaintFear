using UnityEngine;
using System.IO;

namespace FaintFear
{
 
    public static class SaveSystem
    {
        static string SavePath =>
            Path.Combine(Application.persistentDataPath, "save.json");

        // ===================== SAVE =====================
        public static void SaveGame(string checkpointId = "", bool tutorialCompleted = false)
        {
            SaveData prev = LoadPreview();
            SaveData data = new SaveData();

            data.mental = PlayerStatus.Instance.currentMentalPower;
            data.battery = PlayerStatus.Instance.currentBattery;
            data.batteryCount = PlayerStatus.Instance.batteryCount;

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                data.playerPosition = player.transform.position;
                data.playerRotation = player.transform.rotation;
            }

            data.checkpointId = checkpointId;

            // ⭐ 핵심
            data.tutorialCompleted =
                tutorialCompleted || (prev != null && prev.tutorialCompleted);

            // ⭐ 조명 상태 저장 (튜토리얼 완료 시 영구 꺼짐)
            data.lightsPermaOff = data.tutorialCompleted;

            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));

            Debug.Log($"[SaveSystem] Saved to {SavePath}");
        }

        public static SaveData LoadPreview()
        {
            if (!File.Exists(SavePath))
                return null;

            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<SaveData>(json);
        }

        // ===================== LOAD =====================
        public static bool LoadGame()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("[SaveSystem] No save file found");
                return false;
            }

            string json = File.ReadAllText(SavePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // 상태 복원
            PlayerStatus.Instance.SetHealth(data.mental);
            PlayerStatus.Instance.currentBattery = data.battery;
            PlayerStatus.Instance.batteryCount = data.batteryCount;


            // 위치 복원
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = data.playerPosition;
                player.transform.rotation = data.playerRotation;
            }

            Debug.Log("[SaveSystem] Load Complete");
            return true;
        }

        // ===================== CHECK =====================
        public static bool HasSave()
        {
            return File.Exists(SavePath);
        }

        // ===================== DELETE =====================
        public static void DeleteSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
    }
}
