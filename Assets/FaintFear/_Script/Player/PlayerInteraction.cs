using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerInteraction : MonoBehaviour
{
    #region Variables
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform cameraRoot; // 레이 발사 원점 (카메라 위치 권장)

    private PlayerMove playerMove;

    bool isOnLay = false;

    GameObject crossHiair;

    GameObject target;
    #endregion

    #region Unity Event Method
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

    #endregion

    #region Custom Method
    private void ShootRay()
    {
        Vector3 rayOrigin = cameraRoot.position;
        Vector3 rayDirection = cameraRoot.forward;

        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.green, 1f);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, targetLayer))
        {
//            Debug.Log($"범위내에 들어옴: {hit.collider.name}");
            target = hit.transform.gameObject;
            Debug.Log(target);
            crossHiair.SetActive(true);
            isOnLay = true;
        }
        else
        {
            crossHiair.SetActive(false);
            target = null;
            isOnLay = false;
        }
    }

    private void Interact()
    {
        Debug.Log("e키눌림");
        // e키 눌렀을 때 구현
        if(isOnLay && target != null)
        {
            Interactive interactive = target.GetComponentInParent<Interactive>();
            Debug.Log(interactive);
            if (interactive != null)
            {
                interactive.Interaction();
                Debug.Log("실행됨");
            }
            
        }
    }
    #endregion
}
