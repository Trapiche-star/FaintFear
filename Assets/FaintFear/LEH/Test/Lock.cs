using UnityEngine;

public class Lock : MonoBehaviour
{
    [Header("Lock")]
    [SerializeField] private bool isLocked = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    bool isOpen;

    public void Unlock()
    {
        isLocked = false;
    }

    public void TryOpen()
    {
        if (isLocked || isOpen) return;

        isOpen = true;
        animator.SetTrigger("Open");
    }
}
