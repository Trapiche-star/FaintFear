using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isOpen = false;

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        animator.SetTrigger("Open");
    }
}