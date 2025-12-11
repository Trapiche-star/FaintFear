using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class WindowScareEvent : MonoBehaviour
{
    #region Variables
    public Flashlight flashlight;   //나중에 플레이어스탯 싱글톤으로 바꾸기
    private PlayerMove playerMove;

    public CinemachineCamera vcam;
    public Transform playerCamera;          
    public Transform windowLookPoint;       // 창문 바라볼 target
    public float rotateSpeed = 5f;          // 이동 속도

    public GameObject ghost;                // 귀신 오브젝트
    [SerializeField]
    private float ghostSpeed = 10f;         // 귀신 이동 속도
    public Transform moveTarget;            // 귀신이 이동할 타겟

    //시퀀스 텍스트
    public TextMeshProUGUI sequenceText;

    private bool eventTriggered = false;
    #endregion

    #region Unity Event Method
    private void Start()
    {
        playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
    }
    private void Update()
    {
        if(!eventTriggered&& flashlight.CurrentBattery > 0f)
        {
            //딱 한 번만 이벤트 실행
            eventTriggered = true;
            StartCoroutine(SequencePlay());
        }
    }

    #endregion

    #region Custom Method 
    IEnumerator SequencePlay()
    {
        playerMove.enabled = false;
        vcam.enabled = false;
        ghost.SetActive(true);

        //창문으로 강제 시점 이동
        yield return StartCoroutine(LookAtTarget());
        //귀신 지나가기
        yield return StartCoroutine(MoveGhost());
        yield return new WaitForSeconds(1f);

        sequenceText.text = "...방금 뭐였지?";
        yield return new WaitForSeconds(2f);
        sequenceText.text = "";

        playerMove.enabled = true;
        vcam.enabled = true;


        //손전등 튜토리얼 대사 출력
        sequenceText.text = "“[F]를 눌러서 손전등을 켜고 끌 수 있다.";
        yield return new WaitForSeconds(3f);
        sequenceText.text = "";
    }
    //창문으로 강제 시점 이동
    IEnumerator LookAtTarget()
    {
        Quaternion startRot = playerCamera.rotation;
        Vector3 dir = (windowLookPoint.position - playerCamera.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * rotateSpeed;

            playerCamera.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        playerCamera.rotation = targetRot;
    }
    //귀신 지나가기
    IEnumerator MoveGhost()
    {
        while (Vector3.Distance(ghost.transform.position, moveTarget.position) > 0.1f)
        {
            ghost.transform.position = Vector3.MoveTowards( ghost.transform.position, moveTarget.position,
                ghostSpeed * Time.deltaTime);

            yield return null;
        }
        //킬
        Destroy(ghost.gameObject);
    }
    #endregion
}
