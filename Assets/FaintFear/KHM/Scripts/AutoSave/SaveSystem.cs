using UnityEngine;
using System.IO;

namespace FaintFear
{
 
    public static class SaveSystem
    {
        static string SavePath =>
            Path.Combine(Application.persistentDataPath, "save.json");

        // ===================== SAVE =====================
        public static void SaveGame(string checkpointId = "")
        {
            SaveData data = new SaveData();

            // 플레이어 상태
            data.mental = PlayerStatus.Instance.currentMentalPower;
            data.battery = PlayerStatus.Instance.currentBattery;

            // 플레이어 위치
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                data.playerPosition = player.transform.position;
                data.playerRotation = player.transform.rotation;
            }

            // 체크포인트
            data.checkpointId = checkpointId;
            data.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // JSON 변환
            string json = JsonUtility.ToJson(data, true);

            File.WriteAllText(SavePath, json);

            Debug.Log($"[SaveSystem] Saved to {SavePath}");
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
