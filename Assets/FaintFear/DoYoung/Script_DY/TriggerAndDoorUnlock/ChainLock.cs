using FaintFear;
using System.Collections;
using UnityEngine;

public class ChainLock : LockedDoorBase, IActionProvider, ISaveableWorldObject
{
    [SerializeField] private Transform hinge;
    [SerializeField] private GameObject chainRoot;
    [SerializeField] private float openAngle = -90f;

    // ⭐ InstanceID 대신 uniqueId 기반으로 체인 ID 생성
    private string ChainID => $"{uniqueId}_chain";

    protected override bool CanUnlock()
    {
        if (chainRoot == null || !chainRoot.activeSelf) return true;

        if (PuzzleInventory.Instance != null && PuzzleInventory.Instance.HasBoltCutter)
        {
            chainRoot.SetActive(false);

            //+ 체인 절단 SFX 재생
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("SFX_CutChain");

            // ⭐ 체인 비활성화 상태 기록
            RuntimeStateManager.RecordDestroyedObject(ChainID);
            Debug.Log($"[ChainLock] 체인 제거: {ChainID}");

            return true;
        }

        return false;
    }

    protected override void ToggleDoor()
    {
        StartCoroutine(RotateDoor(isOpen ? 0f : openAngle));
        isOpen = !isOpen;

        // 문 상태 런타임 기록
        RuntimeStateManager.RecordDoorState(GetID(), isOpen, isLocked: false);
    }

    private IEnumerator RotateDoor(float targetAngle)
    {
        isMoving = true;
        float elapsed = 0f;
        float duration = 1f;

        Quaternion startRot = hinge.localRotation;
        Quaternion targetRot = Quaternion.Euler(0, targetAngle, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            hinge.localRotation = Quaternion.Lerp(startRot, targetRot, elapsed / duration);
            yield return null;
        }

        hinge.localRotation = targetRot;
        isMoving = false;
    }

    protected override void ApplyDoorRotation()
    {
        if (hinge != null)
            hinge.localRotation = Quaternion.Euler(0, isOpen ? openAngle : 0, 0);
    }

    public string GetActionText()
    {
        return isOpen ? "[E] 문 닫기" : "[E] 문 열기";
    }

    // ===================== ISaveableWorldObject =====================

    public override void Save(ref SaveData data)
    {
        // 부모 클래스의 문 상태 저장 (isOpen, isLocked)
        base.Save(ref data);

        // ⭐ 체인 상태 추가 저장
        if (chainRoot != null && !chainRoot.activeSelf)
        {
            if (!data.destroyedObjects.Contains(ChainID))
            {
                data.destroyedObjects.Add(ChainID);
                Debug.Log($"[ChainLock] Save: 체인 상태 저장 - {ChainID}");
            }
        }
    }

    public override void Load(SaveData data)
    {
        // 부모 클래스의 문 상태 로드
        base.Load(data);

        // ⭐ 체인 상태 로드
        bool chainDestroyed = data.destroyedObjects.Contains(ChainID);

        if (chainRoot != null)
        {
            chainRoot.SetActive(!chainDestroyed);
            Debug.Log($"[ChainLock] Load: 체인 상태 복원 - {ChainID}, Active: {!chainDestroyed}");

            // 런타임 상태에도 반영
            if (chainDestroyed)
            {
                RuntimeStateManager.RecordDestroyedObject(ChainID);
            }
        }
    }
}