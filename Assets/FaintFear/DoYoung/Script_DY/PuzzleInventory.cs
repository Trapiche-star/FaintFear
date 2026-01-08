using UnityEngine;

namespace FaintFear
{
    public class PuzzleInventory : MonoBehaviour, ISaveableWorldObject
    {
        #region Variables

        public static PuzzleInventory Instance;

        private bool[] ownedLevers = new bool[4];
        private bool hasBoltCutter = false;
        private bool hasHook = false;

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

            // ⭐ Awake에서 기본 초기화
            ResetInventory();
        }

        #endregion

        #region Custom Method

        public bool HasAnyLever()
        {
            for (int i = 0; i < ownedLevers.Length; i++)
            {
                if (ownedLevers[i]) return true;
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

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("SFX_Pickup");

            Debug.Log($"[PuzzleInventory] 레버 {leverIndex} 획득");
        }

        public bool ConsumeLever(int leverIndex)
        {
            if (!HasLever(leverIndex))
                return false;

            ownedLevers[leverIndex] = false;
            Debug.Log($"[PuzzleInventory] 레버 {leverIndex} 소모");
            return true;
        }

        public void AcquireBoltCutter()
        {
            if (hasBoltCutter) return;

            hasBoltCutter = true;

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("SFX_Pickup");

            Debug.Log("[PuzzleInventory] 볼트커터 획득");
        }

        public void AcquireHook()
        {
            if (hasHook) return;

            hasHook = true;

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("SFX_Pickup");

            Debug.Log("[PuzzleInventory] 후크 획득");
        }

        // ⭐ 인벤토리 완전 초기화
        public void ResetInventory()
        {
            for (int i = 0; i < ownedLevers.Length; i++)
            {
                ownedLevers[i] = false;
            }

            hasBoltCutter = false;
            hasHook = false;

            Debug.Log("[PuzzleInventory] 인벤토리 초기화 완료");
        }

        #endregion

        #region Property

        public bool HasBoltCutter => hasBoltCutter;
        public bool HasHook => hasHook;

        #endregion

        #region ISaveableWorldObject

        public string GetID() => "PuzzleInventory";

        public void Save(ref SaveData data)
        {
            for (int i = 0; i < ownedLevers.Length; i++)
            {
                data.ownedLevers[i] = ownedLevers[i];
            }

            data.hasBoltCutter = hasBoltCutter;
            data.hasHook = hasHook;

            Debug.Log($"[PuzzleInventory] 저장 - " +
                     $"레버: [{string.Join(", ", ownedLevers)}], " +
                     $"볼트커터: {hasBoltCutter}, 후크: {hasHook}");
        }

        public void Load(SaveData data)
        {
            // ⭐ 핵심: NewGame일 때는 세이브 데이터로 덮어쓰지 않는다
            if (GameManager.Instance != null && GameManager.Instance.IsNewGame)
            {
                Debug.Log("[PuzzleInventory] NewGame 상태 - 로드 무시 (초기화 유지)");
                return;
            }

            for (int i = 0; i < ownedLevers.Length && i < data.ownedLevers.Length; i++)
            {
                ownedLevers[i] = data.ownedLevers[i];
            }

            hasBoltCutter = data.hasBoltCutter;
            hasHook = data.hasHook;

            Debug.Log($"[PuzzleInventory] 로드 - " +
                     $"레버: [{string.Join(", ", ownedLevers)}], " +
                     $"볼트커터: {hasBoltCutter}, 후크: {hasHook}");
        }

        #endregion
    }
}
