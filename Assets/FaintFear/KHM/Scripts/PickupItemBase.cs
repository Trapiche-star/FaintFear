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
        }

        protected abstract void OnPickup();

        public void Save(ref SaveData data)
        {
            if (!gameObject.activeSelf)
                data.destroyedObjects.Add(uniqueId);
        }

        public void Load(SaveData data)
        {
            if (data.destroyedObjects.Contains(uniqueId))
                gameObject.SetActive(false);
        }
    }
}