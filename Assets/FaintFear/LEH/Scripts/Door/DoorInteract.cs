using NavKeypad;
using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    [SerializeField] private SlidingDoor door;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool canOpen = false;
    private bool playerLooking;

    // 키패드에서 호출
    public void EnableDoor()
    {
        canOpen = true;
    }

    void Update()
    {
        if (!canOpen || !playerLooking) return;

        if (Input.GetKeyDown(interactKey))
        {
            door.OpenDoor();
        }
    }

    // KeypadInteractionFPV의 Raycast를 활용
    public void SetLooking(bool value)
    {
        playerLooking = value;
    }
}
