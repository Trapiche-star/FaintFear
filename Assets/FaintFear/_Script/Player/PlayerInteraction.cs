using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FaintFear
{
    /// <summary>
    /// 카메라 시점에서 레이를 발사하여 상호작용 가능한 오브젝트 감지 및 상호작용 처리
    /// </summary>
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

            // 인스펙터에서 직접 연결했다면 이건 건너뜀
            if (cameraRoot == null)
                cameraRoot = GetComponentInChildren<Camera>().transform;

            crossHiair = transform.GetChild(1).GetChild(0).gameObject;


            // crossHiair만 꺼지게 함 (카메라 구조에 영향 X)
            if (crossHiair != null)
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
                target = hit.transform.gameObject;
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
            if (isOnLay && target != null)
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
}