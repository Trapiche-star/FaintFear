using UnityEngine;

namespace FaintFear
{
    public class PuzzleInventory : MonoBehaviour, ISaveableWorldObject
    {
        #region Singleton

        public static PuzzleInventory Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        #endregion

        #region Variables

        private bool[] ownedLevers = new bool[4];
        private bool hasBoltCutter = false;

        #endregion

        #region Custom Method

        public bool HasAnyLever()
        {
            for (int i = 0; i < ownedLevers.Length; i++)
            {
                if (ownedLevers[i])
                    return true;
            }
            return false;
        }

        public bool HasLever(int leverIndex)
        {
            if (leverIndex < 0 || leverIndex >= ownedLevers.Length)
                return false;

            return ownedLevers[leverIndex];
        }

        public void AddLever(int leverIndex)
        {
            if (leverIndex < 0 || leverIndex >= ownedLevers.Length)
                return;

            ownedLevers[leverIndex] = true;
        }

        public bool ConsumeLever(int leverIndex)
        {
            if (!HasLever(leverIndex))
                return false;

            ownedLevers[leverIndex] = false;
            return true;
        }

        public void AcquireBoltCutter()
        {
            if (hasBoltCutter)
                return;

            hasBoltCutter = true;
        }

        #endregion

        #region Property

        public bool HasBoltCutter
        {
            get { return hasBoltCutter; }
        }

        #endregion

        // ⭐ ISaveableWorldObject 구현
        public string GetID() => "PuzzleInventory";

        public void Save(ref SaveData data)
        {
            // 레버 상태 저장
            for (int i = 0; i < ownedLevers.Length; i++)
            {
                data.ownedLevers[i] = ownedLevers[i];
            }

            // 볼트 커터 저장
            data.hasBoltCutter = hasBoltCutter;
        }

        public void Load(SaveData data)
        {
            // 레버 상태 로드
            for (int i = 0; i < ownedLevers.Length && i < data.ownedLevers.Length; i++)
            {
                ownedLevers[i] = data.ownedLevers[i];
            }

            // 볼트 커터 로드
            hasBoltCutter = data.hasBoltCutter;
        }
    }
}