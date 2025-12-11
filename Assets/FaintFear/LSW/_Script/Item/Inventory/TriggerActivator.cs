using UnityEngine;

public class TriggerActivator : MonoBehaviour
{
    public string requiredItem;   // 필요한 아이템 이름
    private Collider col;

    void Start()
    {
        col = GetComponent<Collider>();
        col.isTrigger = false;    // 처음엔 비활성화
    }

    void Update()
    {
        if (Inventory.Instance.HasItem(requiredItem))
        {
            col.isTrigger = true; // 아이템이 있으면 트리거 활성화
        }
    }
}