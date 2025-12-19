using UnityEngine;

public class CameraZoomOnPush : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;

    [Header("Zoom Settings")]
    public float zoomFOV = 40f;      // 확대 시 시야
    public float zoomSpeed = 8f;     // 줌 속도

    private float defaultFOV;
    private bool isTouchingPushItem;

    void Start()
    {
        // 시작 시 기본 FOV 저장
        defaultFOV = playerCamera.fieldOfView;
    }

    void Update()
    {
        bool isPressingV = Input.GetKey(KeyCode.V);

        // 조건: PushItem 접촉 + V키 누르는 중
        if (isTouchingPushItem && isPressingV)
        {
            ZoomIn();
        }
        else
        {
            ZoomOut();
        }
    }

    void ZoomIn()
    {
        playerCamera.fieldOfView =
            Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, Time.deltaTime * zoomSpeed);
    }

    void ZoomOut()
    {
        playerCamera.fieldOfView =
            Mathf.Lerp(playerCamera.fieldOfView, defaultFOV, Time.deltaTime * zoomSpeed);
    }

    // PushItem 접촉 감지 (CharacterController 전용)
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("PushItem"))
        {
            isTouchingPushItem = true;
        }
    }

    // 접촉 해제
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("PushItem"))
        {
            isTouchingPushItem = false;
        }
    }
}