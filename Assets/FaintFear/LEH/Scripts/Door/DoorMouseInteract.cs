using NavKeypad;
using UnityEngine;

public class DoorMouseInteract : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SlidingDoor door;
    [SerializeField] private DoorUnlockController unlockController;
    [SerializeField] private GameObject openUI;

    [Header("Settings")]
    [SerializeField] private KeyCode openKey = KeyCode.E;

    bool isMouseOver;

    void Start()
    {
        openUI.SetActive(false);
    }

    void Update()
    {
        if (!isMouseOver) return;
        if (!unlockController.isUnlocked) return;

        if (Input.GetKeyDown(openKey))
        {
            door.OpenDoor();
            openUI.SetActive(false);
        }
    }

    void OnMouseEnter()
    {
        if (!unlockController.isUnlocked) return;
        isMouseOver = true;
        openUI.SetActive(true);
    }

    void OnMouseExit()
    {
        isMouseOver = false;
        openUI.SetActive(false);
    }
}
