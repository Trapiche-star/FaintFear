using UnityEngine;

namespace FaintFear
{
    public abstract class PickupItemBase : Interactive, ISaveableWorldObject
    {
        [SerializeField] private string uniqueId;

        public string GetID() => uniqueId;

        public override void Interaction()
        {
            OnPickup();
            gameObject.SetActive(false);

            // ⭐ 런타임 상태에 기록
            RuntimeStateManager.RecordDestroyedObject(uniqueId);

            Debug.Log($"[PickupItem] {uniqueId} 획득 - 비활성화 및 런타임 기록");
        }

        protected abstract void OnPickup();

        public void Save(ref SaveData data)
        {
            // 현재 비활성화 상태라면 저장
            if (!gameObject.activeSelf)
            {
                // ⭐ 중복 제거 후 추가
                if (!data.destroyedObjects.Contains(uniqueId))
                {
                    data.destroyedObjects.Add(uniqueId);
                    Debug.Log($"[PickupItem] {uniqueId} Save - destroyedObjects에 추가");
                }
            }
        }

        public void Load(SaveData data)
        {
            // 저장된 상태에서 비활성화되어 있었다면 비활성화
            if (data.destroyedObjects.Contains(uniqueId))
            {
                Debug.Log($"[PickupItem] {uniqueId} Load - 비활성화 시도 (현재 active: {gameObject.activeSelf})");
                gameObject.SetActive(false);
                Debug.Log($"[PickupItem] {uniqueId} Load - 비활성화 완료 (현재 active: {gameObject.activeSelf})");
            }
            else
            {
                Debug.Log($"[PickupItem] {uniqueId} Load - destroyedObjects에 없음, 활성 상태 유지");
            }
        }
    }
}