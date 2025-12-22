using UnityEngine;
using UnityEngine.InputSystem;

namespace FaintFear
{
    public class PushItemAimController : MonoBehaviour
    {
        [Header("Detect")]
        public float detectDistance = 2.5f;
        public LayerMask pushItemLayer;

        [Header("Zoom")]
        public float zoomFOV = 35f;
        public float zoomSpeed = 8f;

        [Header("Player")]
        public PlayerMove playerMove;

        Camera mainCam;
        float defaultFOV;
        bool isZooming;

        void Awake()
        {
            mainCam = Camera.main;
            defaultFOV = mainCam.fieldOfView;
        }

        void Update()
        {
            bool canPush = DetectPushItem();
            bool pressingV = Keyboard.current.vKey.isPressed;

            if (canPush && pressingV)
                StartZoom();
            else
                StopZoom();

            UpdateZoom();
        }

        bool DetectPushItem()
        {
            Collider[] hits = Physics.OverlapSphere
                (transform.position, detectDistance, pushItemLayer);

            return hits.Length > 0;
        }

        void StartZoom()
        {
            if (isZooming) return;

            isZooming = true;
            playerMove.canMove = false;
            playerMove.SetLookLock(true);
        }

        void StopZoom()
        {
            if (!isZooming) return;

            isZooming = false;
            playerMove.canMove = true;
            playerMove.SetLookLock(false);
        }

        void UpdateZoom()
        {
            float targetFOV = isZooming ? zoomFOV : defaultFOV;
            mainCam.fieldOfView =
                Mathf.Lerp(mainCam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        }
    }
}
