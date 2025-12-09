using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform cameraRoot; // 레이 발사 원점 (카메라 위치 권장)

    private PlayerMove playerMove;

    private void Awake()
    {
        // 같은 오브젝트에 있는 PlayerMove 컴포넌트 가져오기
        playerMove = GetComponent<PlayerMove>();

        if (cameraRoot == null) cameraRoot = transform.GetChild(0);
    }

    private void OnEnable()
    {
        if (playerMove != null)
            playerMove.OnInteractEvent += ShootRay;
    }

    private void OnDisable()
    {
        if (playerMove != null)
            playerMove.OnInteractEvent -= ShootRay;
    }

    // E키가 눌렸을 때 실행될 함수
    private void ShootRay()
    {
        Vector3 rayOrigin = cameraRoot.position;
        Vector3 rayDirection = cameraRoot.forward;

        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.green, 1f);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, targetLayer))
        {
            Debug.Log($"상호작용 성공: {hit.collider.name}");

            // 여기에 문 열기, 아이템 줍기 등의 로직 추가
        }
    }
}
