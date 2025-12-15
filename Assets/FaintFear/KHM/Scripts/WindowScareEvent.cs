using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 트리거에 의한 조명제어, 귀신 오브젝트(임시)
    /// 시네머신 카메라 제어를 통한 창문 귀신 이벤트 시퀀스 재생
    /// </summary>
    public class WindowScareEvent : MonoBehaviour
    {
        #region Variables
        private PlayerMove playerMove;              // PlayerMove 스크립트 참조
        public LightZone01 lightZone;               // 조명 제어 스크립트
        public TriggerRestrict triggerRestrict;     // 트리거 제한 제어 스크립트

        public CinemachineCamera vcam;              // 시네머신 카메라
        public Transform playerCamera;              // 플레이어 카메라 트랜스폼
        public Transform windowLookPoint;           // 창문을 바라볼 목표 지점
        public float rotateSpeed = 5f;              // 시점 회전 속도

        public GameObject ghost;                    // 귀신 오브젝트
        [SerializeField]
        private float ghostSpeed = 10f;             // 귀신 이동 속도
        public Transform moveTarget;                // 귀신 이동 목표 지점

        public SequenceTextManager sequenceText;    // 시퀀스 텍스트 관리자
        private string dialogueLine01 = "...방금 뭐였지?";
        private string dialogueLine02 = "[F]를 눌러서 손전등을 켜고 끌 수 있다.";

        private bool eventTriggered = false;        // 이벤트가 이미 실행되었는지 여부
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // Player 오브젝트에서 PlayerMove 스크립트 가져오기
            playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
        }

        private void Update()
        {
            // 이벤트가 아직 실행되지 않았고 배터리가 남아 있을 때 실행
            if (!eventTriggered && PlayerStatus.Instance.currentBattery > 0f)
            {
                eventTriggered = true; // 한 번만 실행하도록 설정
                StartCoroutine(SequencePlay());
            }
        }
        #endregion

        #region Custom Method 
        // 전체 시퀀스 재생 코루틴
        IEnumerator SequencePlay()
        {
            playerMove.enabled = false;     // 플레이어 이동 비활성화
            vcam.enabled = false;           // 시네머신 비활성화
            ghost.SetActive(true);          // 귀신 활성화

            // 시점을 창문 쪽으로 이동
            yield return StartCoroutine(LookAtTarget());
            // 귀신 이동 연출
            yield return StartCoroutine(MoveGhost());
            yield return new WaitForSeconds(0.5f);

            // 첫 번째 대사 출력
            sequenceText.gameObject.SetActive(true);
            sequenceText.ShowMessage(dialogueLine01);
            yield return new WaitForSeconds(2f);

            // 플레이어 조작 및 카메라 다시 활성화
            playerMove.enabled = true;
            vcam.enabled = true;

            // 조명 끄기
            lightZone.SetLightsActive(false);
            // 이동 제한 해제
            triggerRestrict.SetRestriction(false);

            // 손전등 튜토리얼 대사 출력
            sequenceText.gameObject.SetActive(true);
            sequenceText.ShowMessage(dialogueLine02);
        }

        // 카메라를 창문 쪽으로 회전시키는 코루틴
        IEnumerator LookAtTarget()
        {
            Quaternion startRot = playerCamera.rotation;   // 시작 회전값 저장
            Vector3 dir = (windowLookPoint.position - playerCamera.position).normalized; // 목표 방향 계산
            Quaternion targetRot = Quaternion.LookRotation(dir); // 목표 회전값 계산

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * rotateSpeed; // 보간 시간 증가
                playerCamera.rotation = Quaternion.Slerp(startRot, targetRot, t); // 회전 보간
                yield return null;
            }

            playerCamera.rotation = targetRot; // 최종 회전값 적용
        }

        // 귀신이 이동 목표 지점으로 이동하는 코루틴
        IEnumerator MoveGhost()
        {
            while (Vector3.Distance(ghost.transform.position, moveTarget.position) > 0.1f)
            {
                ghost.transform.position = Vector3.MoveTowards(
                    ghost.transform.position,
                    moveTarget.position,
                    ghostSpeed * Time.deltaTime); // 일정 속도로 이동
                yield return null;
            }

            // 목표 도착 후 귀신 오브젝트 제거
            Destroy(ghost.gameObject);
        }
        #endregion
    }
}
