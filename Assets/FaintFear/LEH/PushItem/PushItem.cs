using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushItem : MonoBehaviour
{
    public float pushForce = 1.5f;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearDamping = 4f; // 무거운 느낌
    }

    public void Push(Vector3 dir)
    {
        rb.AddForce(dir * pushForce, ForceMode.Force);
    }
}