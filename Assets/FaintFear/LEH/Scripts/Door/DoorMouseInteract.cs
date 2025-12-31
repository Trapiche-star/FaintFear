using UnityEngine;

public class DoorMouseInteract : MonoBehaviour
{
    [SerializeField] private LockedSlidingDoor door;
    [SerializeField] private GameObject openUI;

    void Start()
    {
        openUI.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (door.IsUnlocked && !door.IsOpen)
            openUI.SetActive(true);
    }

    void OnMouseExit()
    {
        openUI.SetActive(false);
    }

    void Update()
    {
        if (openUI.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            door.TryOpen();
            openUI.SetActive(false);
        }
    }
}
