using UnityEngine;

public class Interactor : MonoBehaviour
{
    public float interactDistance = 2f;
    public LayerMask interactLayer;
    public CrosshairUI crosshairUI;
    public KeyCode interactKey = KeyCode.E;

    private Camera cam;
    private IInteractable currentTarget;

    void Start()
    {
        cam = Camera.main;    
    }
    void Update()
    {
        Ray ray = new(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            IInteractable target = hit.collider.GetComponent<IInteractable>();


            if (target != null)
            {
                //테두리 켜기
                crosshairUI.ShowBorder();
                currentTarget = target;

                if (Input.GetKeyDown(interactKey))
                {
                    //상호작용 호출
                    target.Interact();

                    // 상호작용 완료 이벤트 연결
                    target.OnComplete += OnInteractionComplete;
                }
                return;
            }
        }
        //레이가 아무 것도 안 맞으면 테두리 끄기
        crosshairUI.HideBorder();
        currentTarget = null;
    }
    void OnInteractionComplete()
    {
        //상호작용 끝난 직후 테두리 비활성화
        crosshairUI.HideBorder();

        //이벤트 연결 해제(메모리 누수 방지)
        if(currentTarget != null)
        currentTarget.OnComplete -= OnInteractionComplete;
    }
}
