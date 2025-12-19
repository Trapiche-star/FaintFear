using UnityEngine;

namespace FaintFear
{
    public class CameraZoomController : MonoBehaviour
    {
        [Header("Reference")]
        public PowerGaugePlayer powerGauge;

        [Header("Zoom Settings")]
        public float zoomFOV = 40f;
        public float zoomSpeed = 5f;

        [Header("Rotate Settings")]
        public float rotateSpeed = 5f;

        Camera cam;
        float defaultFOV;

        void Awake()
        {
            cam = GetComponent<Camera>();
            defaultFOV = cam.fieldOfView;
        }

        void Update()
        {
            if (powerGauge == null)
                return;

            HandleZoom();
        }

        void HandleZoom()
        {
            bool canZoom = powerGauge.CanZoom;

            // FOV 줌
            float targetFOV = canZoom ? zoomFOV : defaultFOV;
            cam.fieldOfView = Mathf.Lerp(
                cam.fieldOfView,
                targetFOV,
                Time.deltaTime * zoomSpeed
            );

            // PushItem 방향으로 회전
            if (canZoom && powerGauge.ZoomTarget != null)
            {
                Vector3 dir =
                    powerGauge.ZoomTarget.position - transform.position;

                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    Time.deltaTime * rotateSpeed
                );
            }
        }
    }
}