using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FaintFear
{
<<<<<<< HEAD
    /// <summary>
    /// 카메라 시점에서 레이를 발사하여 상호작용 가능한 오브젝트 감지 및 상호작용 처리
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
=======
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
>>>>>>> 15ef700b01aac956649522236d4e3bd91c13b319
    {
        [SerializeField] private float rayDistance = 2f;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private Transform cameraRoot; // 레이 발사 원점 (카메라 위치 권장)

        private PlayerMove playerMove;

        bool isOnLay = false;

        GameObject crossHiair;

<<<<<<< HEAD
        private void Awake()
        {

            // 같은 오브젝트에 있는 PlayerMove 컴포넌트 가져오기
            playerMove = GetComponent<PlayerMove>();

            // 인스펙터에서 직접 연결했다면 이건 건너뜀
            if (cameraRoot == null)
                cameraRoot = GetComponentInChildren<Camera>().transform;

            // 이름으로 정확히 CrossHair만 찾기 (하드 인덱스 사용 금지)
            crossHiair = GameObject.Find("CrossHair");

            // crossHiair만 꺼지게 함 (카메라 구조에 영향 X)
            if (crossHiair != null)
                crossHiair.SetActive(false);
=======
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
>>>>>>> 15ef700b01aac956649522236d4e3bd91c13b319
        }

        private void OnEnable()
        {
<<<<<<< HEAD
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
                isOnLay = true;
            }
            else
            {
                crossHiair.SetActive(false);
                isOnLay = false;
            }
        }

        private void Interact()
        {
            // e키 눌렀을 때 구현
            if (!isOnLay)
            {

            }
        }
    }

=======
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
>>>>>>> 15ef700b01aac956649522236d4e3bd91c13b319
}
