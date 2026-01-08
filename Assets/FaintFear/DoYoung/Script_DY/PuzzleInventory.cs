using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 퍼즐 진행에 필요한 영구 도구 및 상태를 관리하는 인벤토리
    /// 레버, 볼트커터, 후크 보유 여부를 저장 및 로드한다
    /// </summary>
    public class PuzzleInventory : MonoBehaviour, ISaveableWorldObject
    {
        #region Variables

        public static PuzzleInventory Instance;   // 싱글톤 인스턴스

        private bool[] ownedLevers = new bool[4]; // 레버 보유 상태
        private bool hasBoltCutter = false;       // 볼트커터 보유 여부
        private bool hasHook = false;             // 후크 보유 여부

        #endregion


        #region Unity Event Method

        // 싱글톤 인스턴스를 설정한다
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return; // 만약 기존 인스턴스가 있다면 이 오브젝트는 더 이상 유지하지 않는다
            }

            Instance = this;
        }

        #endregion


        #region Custom Method

        // 하나라도 레버를 보유 중인지 검사한다
        public bool HasAnyLever()
        {
            for (int i = 0; i < ownedLevers.Length; i++) // 만약 모든 레버를 순회한다면 보유 여부를 하나씩 확인한다
            {
                if (ownedLevers[i]) return true;         // 만약 하나라도 보유 중이라면 이 메서드에서는 더 이상 검사하지 않는다
            }
            return false;
        }

        // 특정 인덱스의 레버 보유 여부를 반환한다
        public bool HasLever(int leverIndex)
        {
            if (leverIndex < 0 || leverIndex >= ownedLevers.Length)
                return false; // 만약 인덱스가 범위를 벗어난다면 이 메서드에서는 더 이상 판정하지 않는다

            return ownedLevers[leverIndex];
        }

        // 레버를 인벤토리에 추가한다
        public void AddLever(int leverIndex)
        {
            if (leverIndex < 0 || leverIndex >= ownedLevers.Length)
                return; // 만약 인덱스가 범위를 벗어난다면 이 메서드에서는 더 이상 처리하지 않는다

            ownedLevers[leverIndex] = true;

            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX("SFX_Pickup"); // 만약 사운드 매니저가 있다면 픽업 사운드를 재생한다
        }

        // 레버를 소모 처리한다
        public bool ConsumeLever(int leverIndex)
        {
            if (!HasLever(leverIndex))
                return false; // 만약 해당 레버가 없다면 이 메서드에서는 더 이상 소모하지 않는다

            ownedLevers[leverIndex] = false;
            return true;
        }

        // 볼트커터를 획득 처리한다
        public void AcquireBoltCutter()
        {
            if (hasBoltCutter) return; // 만약 이미 보유 중이라면 이 메서드에서는 더 이상 처리하지 않는다

            hasBoltCutter = true;

            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX("SFX_Pickup"); // 만약 사운드 매니저가 있다면 픽업 사운드를 재생한다
        }

        // 후크를 획득 처리한다
        public void AcquireHook()
        {
            if (hasHook) return; // 만약 이미 보유 중이라면 이 메서드에서는 더 이상 처리하지 않는다

            hasHook = true;

            if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX("SFX_Pickup"); // 만약 사운드 매니저가 있다면 픽업 사운드를 재생한다
        }

        #endregion


        #region Property

        public bool HasBoltCutter => hasBoltCutter; // 볼트커터 보유 여부를 반환한다
        public bool HasHook => hasHook;             // 후크 보유 여부를 반환한다

        #endregion


        // ISaveableWorldObject 구현

        public string GetID() => "PuzzleInventory";

        // 현재 퍼즐 인벤토리 상태를 저장한다
        public void Save(ref SaveData data)
        {
            for (int i = 0; i < ownedLevers.Length; i++) // 만약 모든 레버 슬롯을 순회한다면 저장 데이터에 상태를 기록한다
            {
                data.ownedLevers[i] = ownedLevers[i];
            }

            data.hasBoltCutter = hasBoltCutter;
            data.hasHook = hasHook;
        }

        // 저장된 퍼즐 인벤토리 상태를 로드한다
        public void Load(SaveData data)
        {
            for (int i = 0; i < ownedLevers.Length && i < data.ownedLevers.Length; i++) // 만약 양쪽 배열 범위 안이라면 해당 인덱스만 안전하게 로드한다
            {
                ownedLevers[i] = data.ownedLevers[i];
            }

            hasBoltCutter = data.hasBoltCutter;
            hasHook = data.hasHook;
        }
    }
}
