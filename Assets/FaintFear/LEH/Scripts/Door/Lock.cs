using UnityEngine;

public enum LockUnlockType
{
    None,
    Puzzle,     // 퍼즐로만 해제 가능
    DoorLock    // 도어락으로만 해제 가능
}

public class Lock : MonoBehaviour
{
    [Header("Lock Settings")]
    [SerializeField] private bool isLocked = true;
    [SerializeField] private LockUnlockType unlockType = LockUnlockType.DoorLock;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    bool isOpen;

    /* ===============================
     * 잠금 해제 (방식 제한)
     * ===============================*/

    //도어락 전용 해제
    public void UnlockByDoorLock()
    {
        if (unlockType != LockUnlockType.DoorLock)
            return;

        isLocked = false;
    }

    //퍼즐 전용 해제 (기존 퍼즐 문용)
    public void UnlockByPuzzle()
    {
        if (unlockType != LockUnlockType.Puzzle)
            return;

        isLocked = false;
    }

    /* ===============================
     * 문 열기 (플레이어 상호작용)
     * ===============================*/
    public void TryOpen()
    {
        if (isLocked)
        {
            Debug.Log("문이 잠겨 있습니다.");
            return;
        }

        if (isOpen) return;

        isOpen = true;
        animator.SetTrigger("Open");
    }
}
