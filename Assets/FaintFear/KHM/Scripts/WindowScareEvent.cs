using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

namespace FaintFear
{
    public class WindowScareEvent : MonoBehaviour
    {
        #region Variables
        private PlayerMove playerMove;
        private Flashlight flashlight;
        public LightZone01 lightZone;
        public TriggerRestrict triggerRestrict;

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
        private string dialogueLine01 = "[F]를 눌러서 손전등을 켜고 끌 수 있다.";
        private string dialogueLine02 = "어둠에 노출될 때마다 비정상적인 공포심이 몰려든다...";
        private string dialogueLine03 = "빛에서 멀어지지 않는게 좋겠다.";
        


        private bool eventTriggered = false;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
            flashlight = GameObject.FindAnyObjectByType<Flashlight>();
        }
        private void Update()
        {
            if (!eventTriggered && PlayerStatus.Instance.currentBattery > 0f)
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
            //플레이어 고정
            playerMove.canMove = false;
            vcam.enabled = false;

            //조명 끄기
            lightZone.SetLightsActive(false);

            //손전등 튜토리얼 대사 출력
            sequenceText.gameObject.SetActive(true);
            sequenceText.text = dialogueLine01;

            //손전등을 켰을 때 
            if(flashlight.IsOn)
            {
                sequenceText.text = "";

                ghost.SetActive(true);

                //창문으로 강제 시점 이동
                yield return StartCoroutine(LookAtTarget());
                //귀신 지나가기
                yield return StartCoroutine(MoveGhost());
                yield return new WaitForSeconds(0.5f);

                //플레이어 움직임 고정 해제
                playerMove.canMove = true;
                vcam.enabled = true;

                //이탈 제한 구역 콜라이더, 트리거 해제
                triggerRestrict.SetRestriction(false);

                //텍스트 출력
                sequenceText.gameObject.SetActive(true);
                sequenceText.text = dialogueLine02;
                yield return new WaitForSeconds(2f);
                sequenceText.text = dialogueLine03;
                yield return new WaitForSeconds(2f);
                sequenceText.text = "";

                //정신력 시스템 활성화
                PlayerStatus.Instance.isMentalSystemActive = true;
            }
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
                ghost.transform.position = Vector3.MoveTowards(ghost.transform.position, moveTarget.position,
                    ghostSpeed * Time.deltaTime);

                yield return null;
            }
            //킬
            Destroy(ghost.gameObject);
        }
        #endregion
    }
}