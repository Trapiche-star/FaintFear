using System.Collections;
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 배터리 획득 후 창문 연출, 시선 고정, 귀신 이벤트를 처리하는 시퀀스 이벤트
    /// </summary>
    public class WindowScareEvent : TutorialEventBase
    {
        #region Variables

        [SerializeField] private LightZone01 lightZone;
        [SerializeField] private TriggerRestrict triggerRestrict;

        [Header("카메라 연출")]
        [SerializeField] private Transform windowLookPoint;
        [SerializeField] private float rotateSpeed = 5f;

        [Header("귀신 연출")]
        [SerializeField] private GameObject ghost;
        [SerializeField] private Transform moveTarget;
        [SerializeField] private float ghostSpeed = 10f;

        [Header("텍스트")]
        [SerializeField] private SequenceTextManager sequenceText;

        private readonly string dialogueLine01 = "[F]를 눌러서 손전등을 켜고 끌 수 있다.";
        private readonly string dialogueLine02 = "어둠에 노출될 때마다 비정상적인 공포심이 몰려든다...";
        private readonly string dialogueLine03 = "빛에서 멀어지지 않는게 좋겠다.";

        [Header("EnemySpawner")]
        public GameObject spawner;

        #endregion

        #region Unity Event Method

        private void Start()
        {
            var data = SaveSystem.LoadPreview();
            bool tutorialDone = data != null && data.tutorialCompleted;

            if (tutorialDone || GameManager.TutorialCompleted)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Update()
        {
            if (!CanPlay()) return;
            if (PlayerStatus.Instance.currentBattery <= 0f) return;

            Play(SequencePlay());
        }

        #endregion

        #region Custom Method

        protected override bool IsTutorialCompleted()
        {
            if (GameManager.TutorialCompleted) return true;

            var data = SaveSystem.LoadPreview();
            return data != null && data.tutorialCompleted;
        }

        // 창문 공포 연출 전체 흐름을 처리하는 메인 시퀀스
        private IEnumerator SequencePlay()
        {
            if (IsTutorialCompleted())
                yield break;

            if (flashlight == null)
            {
                Debug.LogError("[WindowScareEvent] Flashlight not found");
                yield break;
            }

            if (GameManager.TutorialCompleted)
                yield break;

            // 플레이어 이동 잠금
            playerMove.canMove = false;

            // 조명 끄기
            lightZone.SetLightsActive(false);
            lightZone.SetPermanentlyOff();

            //+ 소등 효과음 재생
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX("SFX_LightOff"); //+
            }

            // 손전등 튜토리얼은 지속 출력
            sequenceText.ShowPersistentMessage(dialogueLine01);

            // 손전등 ON까지 대기
            yield return new WaitUntil(() => flashlight.IsOn);

            //+손전등 처음 켰을 때 점프스케어 사운드 재생
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX("SFX_Jumpscare01"); //+
            }

            //+손전등 처음 켰을 때 BGM_Tense 10초 임시 재생
            if (SoundManager.Instance != null)
            {
                StartCoroutine(PlayBGMTemporary("BGM_Tense", 10f)); //+
            }

            if (IsTutorialCompleted())
                yield break;

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

            // 플레이어 조작 복구
            playerMove.enabled = true;
            playerMove.canMove = true;

            // 이동 제한 해제
            triggerRestrict.SetRestriction(false);

            // ⭐ BGM 10초 병렬 재생
            if (SoundManager.Instance != null)
            {
                StartCoroutine(PlayBGMTemporary("BGM_Tense", 10f));
            }

            // 설명 대사
            sequenceText.ShowMessage(dialogueLine02, 2.5f);
            yield return new WaitForSeconds(2.5f);

            sequenceText.ShowMessage(dialogueLine03, 2.5f);
            yield return new WaitForSeconds(2.5f);

            // 정신력 시스템 활성화
            PlayerStatus.Instance.isMentalSystemActive = true;

            // 세이브 포인트 저장
            SaveSystem.SaveGame("TutorialEnd", tutorialCompleted: true);

            // GameManager static 변수 즉시 업데이트
            GameManager.TutorialCompleted = true;

            Debug.Log("[WindowScareEvent] Tutorial completed and saved");

            AutoSaveManager.Instance.RequestSave("TutorialEnd");

            sequenceText.Hide();

            spawner.SetActive(true);
        }

        // 카메라를 창문 방향으로 부드럽게 회전시킨다
        private IEnumerator LookAtTarget()
        {
            if (cameraPosition == null || windowLookPoint == null)
                yield break;

            Quaternion start = cameraPosition.rotation;
            Quaternion target = Quaternion.LookRotation(
                (windowLookPoint.position - cameraPosition.position).normalized
            );

            float t = 0f;
            while (t < 1f)
            {
                if (cameraPosition == null)
                    yield break;

                t += Time.deltaTime * rotateSpeed;
                cameraPosition.rotation = Quaternion.Slerp(start, target, t);
                yield return null;
            }
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

            // 귀신 오브젝트 제거
            Destroy(ghost);
        }

        //+ BGM 10초 병렬 재생 후 이전 BGM으로 복귀
        private IEnumerator PlayBGMTemporary(string bgmName, float duration)
        {
            if (SoundManager.Instance == null)
                yield break;

            //!!! 이전 BGM 저장 (getter 사용)
            var prevBGM = SoundManager.Instance.CurrentBGMName; //!!!

            //!!! 임시 BGM 재생
            SoundManager.Instance.PlayBGM(bgmName, rememberPrevious: false); //!!!

            // duration 동안 대기
            yield return new WaitForSeconds(duration);

            //!!! 이전 BGM 복귀
            if (!string.IsNullOrEmpty(prevBGM))
            {
                SoundManager.Instance.PlayBGM(prevBGM, rememberPrevious: false); //!!!
            }
            else
            {
                Debug.Log("[WindowScareEvent] 이전 BGM 없음, BGM 종료"); //!!!
            }
        }
    }

        #endregion
    }

