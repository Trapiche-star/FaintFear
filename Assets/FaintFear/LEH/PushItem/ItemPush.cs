using UnityEngine;

public class ItemPush : MonoBehaviour
{
    public float pushForce = 5f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Push(Vector3 direction)
    {
        if (rb == null) return;

        rb.AddForce(direction.normalized * pushForce, ForceMode.Impulse);
    }
}