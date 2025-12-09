using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform cameraRoot; // 레이 발사 원점 (카메라 위치 권장)

    private PlayerMove playerMove;


    GameObject crossHiair;

    private void Awake()
    {
        // 같은 오브젝트에 있는 PlayerMove 컴포넌트 가져오기
        playerMove = GetComponent<PlayerMove>();

        if (cameraRoot == null) cameraRoot = transform.GetChild(0);
        crossHiair = transform.GetChild (1).GetChild(0).gameObject;
        crossHiair.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerMove != null)
            playerMove.OnInteractEvent += Interact;
    }

    private void OnDisable()
    {
        if (playerMove != null)
            playerMove.OnInteractEvent -= Interact;
    }

    private void Update()
    {
        ShootRay();
    }



    private void ShootRay()
    {
        Vector3 rayOrigin = cameraRoot.position;
        Vector3 rayDirection = cameraRoot.forward;

        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.green, 1f);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, targetLayer))
        {
            Debug.Log($"범위내에 들어옴: {hit.collider.name}");
            crossHiair.SetActive(true);

        }
        else
        {
            crossHiair.SetActive(false);

        }
    }

    private void Interact()
    {
        // e키 눌렀을 때 구현
    }
}
