using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 지하실에서 1층으로 이동하는 출구 도어
    /// 문 개방 연출 중 플레이어를 고정하고, 완료 후 씬 이동을 처리한다
    /// </summary>
    public class BasementExitDoor : Interactive, IActionProvider
    {
        #region Variables

        [Header("Scene")]
        [SerializeField] private string targetSceneName;            // 이동할 씬 이름 (Level01)

        [Header("Door")]
        [SerializeField] private Transform doorPivot;               // 회전할 문 피벗
        [SerializeField] private float openedAngle = 90f;           // 문이 완전히 열린 각도
        [SerializeField] private float openSpeed = 90f;             // 문 회전 속도

        [Header("Sequence")]
        [SerializeField] private SequenceTextManager sequenceText;  // 시퀀스 메시지 출력

        private PlayerMove playerMove;   // 플레이어 이동/시점 제어
        private bool isOpened = false;   // 문이 완전히 열렸는지 여부
        private bool isOpening = false;  // 문 개방 연출 중 여부
        private bool pendingUnlock = false; // 씬 로드 후 잠금 해제 예약 여부

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            playerMove = FindAnyObjectByType<PlayerMove>();
            // 씬 내 플레이어 이동 컴포넌트를 참조한다
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            // 씬 로드 완료 이벤트를 구독한다
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            // 씬 로드 완료 이벤트를 해제한다
        }

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            if (isOpening) return;
            // 만약 [문 연출 중이라면] [상호작용을 차단한다]

            if (!isOpened)
            {
                StartCoroutine(OpenDoorAndMoveRoutine());
                return;
                // 만약 [아직 열리지 않았다면] [개방 연출을 시작한다]
            }

            MoveToScene();
            // 만약 [이미 열려 있다면] [씬 이동을 실행한다]
        }

        // 문 개방 연출 → 씬 이동 전체 흐름
        private IEnumerator OpenDoorAndMoveRoutine()
        {
            isOpening = true;
            // 문 개방 연출 시작 상태로 전환한다

            CachePlayer();
            // 플레이어 참조를 최신으로 확보한다

            LockPlayer(true);
            // 문이 열리는 동안 플레이어 이동/시점을 차단한다

            if (sequenceText != null)
                sequenceText.ShowMessage("문을 열었다.");
            // 문 개방 시퀀스 메시지를 출력한다

            yield return new WaitForSeconds(0.3f);
            // 메시지 인지 시간을 잠시 확보한다

            yield return StartCoroutine(OpenDoorRoutine());
            // 문이 완전히 열릴 때까지 대기한다

            isOpened = true;
            isOpening = false;
            // 문 개방 완료 상태로 전환한다

            pendingUnlock = true;
            // 씬 로드 후 잠금 해제를 예약한다

            MoveToScene();
            // 문 개방이 끝난 직후 씬 이동을 실행한다
        }

        // 문을 자동으로 여는 연출
        private IEnumerator OpenDoorRoutine()
        {
            if (doorPivot == null)
                yield break;
            // 만약 [문 피벗이 없다면] [연출을 수행하지 않는다]

            float startAngle = doorPivot.localEulerAngles.y;
            float targetAngle = openedAngle;

            float t = 0f;
            float duration = Mathf.Abs(targetAngle - startAngle) / openSpeed;

            while (t < duration)
            {
                t += Time.deltaTime;

                float angle = Mathf.Lerp(startAngle, targetAngle, t / duration);
                doorPivot.localEulerAngles = new Vector3(0f, angle, 0f);

                yield return null;
            }

            doorPivot.localEulerAngles = new Vector3(0f, targetAngle, 0f);
            // 문이 완전히 열린 상태를 보장한다
        }

        // 씬 로드 완료 시 잠금 해제 처리
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!pendingUnlock) return;
            // 만약 [잠금 해제 예약이 없다면] [아무 처리도 하지 않는다]

            if (scene.name != targetSceneName) return;
            // 만약 [목표 씬이 아니라면] [이 스크립트에서는 처리하지 않는다]

            CachePlayer();
            // 새 씬 기준으로 플레이어 참조를 다시 확보한다

            LockPlayer(false);
            // 씬 전환 완료 후 플레이어 이동/시점을 복구한다

            pendingUnlock = false;
            // 잠금 해제 예약 상태를 해제한다
        }

        // 플레이어 참조를 최신으로 확보한다
        private void CachePlayer()
        {
            if (playerMove != null) return;
            // 만약 [이미 참조가 있다면] [재탐색하지 않는다]

            playerMove = FindAnyObjectByType<PlayerMove>();
            // 씬 내에서 PlayerMove를 다시 찾는다
        }

        // 플레이어 이동 및 시점 고정 처리
        private void LockPlayer(bool locked)
        {
            if (playerMove == null) return;
            // 만약 [플레이어 이동 컴포넌트가 없다면] [처리를 중단한다]

            playerMove.canMove = !locked;
            // 이동 가능 여부를 설정한다

            playerMove.SetLookLock(locked);
            // 시점 회전 고정 여부를 설정한다
        }

        // 씬 이동을 실행한다
        private void MoveToScene()
        {
            if (string.IsNullOrEmpty(targetSceneName)) return;
            // 만약 [씬 이름이 비어 있다면] [이동하지 않는다]

            if (SceneLoadManager.Instance == null) return;
            // 만약 [씬 로드 매니저가 없다면] [요청을 중단한다]

            SceneLoadManager.Instance.LoadScene(targetSceneName);
            // 씬 이동을 매니저에 위임한다
        }

        #endregion


        #region Property

        // Action UI에 표시할 문구를 제공한다
        public string GetActionText()
        {
            if (isOpening)
                return string.Empty;
            // 만약 [연출 중이라면] [액션 UI를 숨긴다]

            return isOpened ? "이동하기" : "문 열기";
            // 상태에 따라 액션 문구를 변경한다
        }

        #endregion
    }
}
