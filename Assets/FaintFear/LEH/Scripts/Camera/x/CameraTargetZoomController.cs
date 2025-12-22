using UnityEngine;

namespace FaintFear
{
    public class CameraTargetZoomController : MonoBehaviour
    {
        public PowerGaugePlayer powerGauge;

        [Header("Zoom")]
        public float zoomFOV = 35f;
        public float zoomSpeed = 5f;

        [Header("Lock")]
        public float rotateSpeed = 8f;

        Camera cam;
        float defaultFOV;
        Quaternion defaultRotation;

        void Awake()
        {
            cam = GetComponent<Camera>();
            defaultFOV = cam.fieldOfView;
            defaultRotation = transform.rotation;
        }

        void LateUpdate()
        {
            if (powerGauge == null) return;

            bool zoom = powerGauge.CanZoom;
            Transform target = powerGauge.ZoomTarget;

            // FOV
            float fov = zoom ? zoomFOV : defaultFOV;
            cam.fieldOfView = Mathf.Lerp(
                cam.fieldOfView,
                fov,
                Time.deltaTime * zoomSpeed
            );

            // 회전 고정 (타겟 바라보기)
            if (zoom && target != null)
            {
                Vector3 dir = target.position - transform.position;
                Quaternion look = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    look,
                    Time.deltaTime * rotateSpeed
                );
            }
            else
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    defaultRotation,
                    Time.deltaTime * rotateSpeed
                );
            }
        }
    }
}
