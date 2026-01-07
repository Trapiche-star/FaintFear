using UnityEngine;

namespace FaintFear
{
    public class CloseUIInput : MonoBehaviour
    {
        [SerializeField] private bool useInteractKey = true;

        private PlayerMove playerMove;
        private PlayerInputAction input;

        private void Awake()
        {
            input = new PlayerInputAction();

            if (useInteractKey)
                input.Player.Interaction.performed += _ => Close();
        }

        private void OnEnable()
        {
            input.Enable();
            CachePlayer();

            if (playerMove != null)
                playerMove.enabled = false;
        }

        private void OnDisable()
        {
            input.Disable();
        }

        private void OnDestroy()
        {
            if (useInteractKey)
                input.Player.Interaction.performed -= _ => Close();
        }

        private void CachePlayer()
        {
            if (playerMove != null) return;
            playerMove = Object.FindFirstObjectByType<PlayerMove>();
        }

        private void Close()
        {
            PickupDocument.Current?.CloseDocument();

            gameObject.SetActive(false);

            CachePlayer();
            if (playerMove != null)
                playerMove.enabled = true;

            if (PlayerStatus.Instance != null)
            {
                PlayerStatus.Instance.isMentalSystemActive = true;
                PlayerStatus.Instance.isBatteryActive = true;
            }
        }
    }
}
