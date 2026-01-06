using FaintFear;
using UnityEngine;

public class PlayerFlashLightInteraction : MonoBehaviour
{
    private PlayerMove playerMove;
    private Flashlight flashlight;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    // ⭐ GameManager에서 플레이어 스폰 후 호출
    public void BindFlashlight(GameObject player)
    {
        flashlight = player.GetComponentInChildren<Flashlight>(true);

        if (flashlight == null)
            Debug.LogError("[PlayerFlashLightInteraction] Flashlight not found");
    }

    private void OnEnable()
    {
        if (playerMove != null)
            playerMove.OnFlashLightEvent += OnFlash;
    }

    private void OnDisable()
    {
        if (playerMove != null)
            playerMove.OnFlashLightEvent -= OnFlash;
    }

    private void OnFlash()
    {
        if (flashlight == null)
        {
            Debug.LogWarning("Flashlight is null");
            return;
        }

        flashlight.ToggleLight();
        Debug.Log("f키 입력");
    }
}
