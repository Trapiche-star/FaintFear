using UnityEngine;

public class DoorUnlockController : MonoBehaviour
{
    public bool isUnlocked { get; private set; }

    public void UnlockDoor()
    {
        isUnlocked = true;
        Debug.Log("문 잠금 해제됨");
    }
}
