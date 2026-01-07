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
        }

        protected abstract void OnPickup();

        public void Save(ref SaveData data)
        {
            // 현재 비활성화 상태라면 저장
            if (!gameObject.activeSelf)
                data.destroyedObjects.Add(uniqueId);
        }

        public void Load(SaveData data)
        {
            // 저장된 상태에서 비활성화되어 있었다면 비활성화
            if (data.destroyedObjects.Contains(uniqueId))
                gameObject.SetActive(false);
        }
    }
}