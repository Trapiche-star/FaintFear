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
            data.tutorialCompleted =
                tutorialCompleted || (prev != null && prev.tutorialCompleted);

            data.lightsPermaOff = data.tutorialCompleted;

            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        }

        public static SaveData LoadPreview()
        {
            if (!File.Exists(SavePath))
                return null;

            return JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        }

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
