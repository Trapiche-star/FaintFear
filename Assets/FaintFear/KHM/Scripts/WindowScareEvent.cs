using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 배터리 획득 후 창문 연출, 시선 고정, 귀신 이벤트를 처리하는 시퀀스 이벤트
    /// </summary>
    public class WindowScareEvent : MonoBehaviour
    {
        #region Variables

        private PlayerMove playerMove;                    // 플레이어 이동 제어 컴포넌트
        private Flashlight flashlight;                    // 손전등 제어 컴포넌트

        [SerializeField] private LightZone01 lightZone;   // 라이트 존 제어기
        [SerializeField] private TriggerRestrict triggerRestrict; // 이동 제한 트리거
        [SerializeField] private CinemachineCamera vcam;  // 시네머신 가상 카메라

        [Header("카메라 연출")]
        [SerializeField] private Transform cameraPosition;    // 회전시킬 카메라 기준
        [SerializeField] private Transform windowLookPoint;   // 창문 시선 타겟
        [SerializeField] private float rotateSpeed = 5f;      // 시선 회전 속도

        [Header("귀신 연출")]
        [SerializeField] private GameObject ghost;            // 귀신 오브젝트
        [SerializeField] private Transform moveTarget;        // 귀신 이동 목표 지점
        [SerializeField] private float ghostSpeed = 10f;      // 귀신 이동 속도

        [Header("텍스트")]
        [SerializeField] private SequenceTextManager sequenceText; // 시퀀스 텍스트 매니저

        private readonly string dialogueLine01 = "[F]를 눌러서 손전등을 켜고 끌 수 있다.";
        private readonly string dialogueLine02 = "어둠에 노출될 때마다 비정상적인 공포심이 몰려든다...";
        private readonly string dialogueLine03 = "빛에서 멀어지지 않는게 좋겠다.";

        private bool eventTriggered = false;               // 이벤트 1회 실행 보장

        #endregion


        #region Unity Event Method

        private void Start()
        {
            if (GameManager.TutorialCompleted)
            {
                Destroy(this);
                return;
            }

            playerMove = GameObject.FindWithTag("Player")?.GetComponent<PlayerMove>();
            flashlight = GameObject.FindAnyObjectByType<Flashlight>();
        }

        private void Update()
        {
            if (GameManager.TutorialCompleted)
                return;

            if (!eventTriggered && PlayerStatus.Instance.currentBattery > 0f)
            {
                eventTriggered = true;
                StartCoroutine(SequencePlay());
            }
        }

        #endregion


        #region Custom Method

        // 창문 공포 연출 전체 흐름을 처리하는 메인 시퀀스
        private IEnumerator SequencePlay()
        {
            if (GameManager.TutorialCompleted)
                yield break;

            // 플레이어 이동 및 카메라 잠금
            playerMove.canMove = false;
            vcam.enabled = false;

            // 조명 끄기
            lightZone.SetLightsActive(false);

            // 손전등 튜토리얼은 지속 출력
            sequenceText.ShowPersistentMessage(dialogueLine01);

            // 손전등 ON까지 대기
            yield return new WaitUntil(() => flashlight.IsOn);

            // 튜토리얼 텍스트 제거
            sequenceText.Hide();

            // 손전등 조작 잠금
            playerMove.enabled = false;

            // 귀신 등장
            ghost.SetActive(true);

            // 시선 고정
            yield return StartCoroutine(LookAtTarget());

            // 귀신 이동
            yield return StartCoroutine(MoveGhost());

            // 잠깐 여유
            yield return new WaitForSeconds(0.5f);

            // 플레이어 조작 복구
            playerMove.enabled = true;
            playerMove.canMove = true;
            vcam.enabled = true;

            // 이동 제한 해제
            triggerRestrict.SetRestriction(false);

            // 설명 대사
            sequenceText.ShowMessage(dialogueLine02, 2.5f);
            yield return new WaitForSeconds(2.5f);

            sequenceText.ShowMessage(dialogueLine03, 2.5f);
            yield return new WaitForSeconds(2.5f);

            // 정신력 시스템 활성화
            PlayerStatus.Instance.isMentalSystemActive = true;

            //세이브 포인트 저장
            SaveSystem.SaveGame("TutorialEnd", tutorialCompleted: true);
            AutoSaveManager.Instance.RequestSave("TutorialEnd");
            
            sequenceText.Hide();
        }

        // 카메라를 창문 방향으로 부드럽게 회전시킨다
        private IEnumerator LookAtTarget()
        {
            if (cameraPosition == null || windowLookPoint == null)
                yield break;

            Quaternion startRot = cameraPosition.rotation;

            Vector3 dir = (windowLookPoint.position - cameraPosition.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * rotateSpeed;          // 시간 기반 보간
                cameraPosition.rotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            cameraPosition.rotation = targetRot;            // 최종 각도 고정
        }

        // 귀신을 목표 지점까지 이동시킨 후 제거한다
        private IEnumerator MoveGhost()
        {
            while (Vector3.Distance(ghost.transform.position, moveTarget.position) > 0.1f)
            {
                ghost.transform.position =
                    Vector3.MoveTowards(
                        ghost.transform.position,
                        moveTarget.position,
                        ghostSpeed * Time.deltaTime
                    );

                yield return null;
            }

            // 귀신 오브젝트를 제거한다
            Destroy(ghost);
        }

        #endregion
    }
}
