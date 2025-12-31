using UnityEngine;

public class LockedSlidingDoor : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public bool IsUnlocked { get; private set; }
    public bool IsOpen { get; private set; }

    public void UnlockDoor()
    {
        IsUnlocked = true;
        Debug.Log("문 잠금 해제됨");
    }

    public void TryOpen()
    {
        if (!IsUnlocked)
        {
            Debug.Log("아직 잠겨 있음");
            return;
        }

        if (IsOpen) return;

        IsOpen = true;
        animator.SetBool("isOpen", true);
    }

    public void Close()
    {
        IsOpen = false;
        animator.SetBool("isOpen", false);
    }
}

