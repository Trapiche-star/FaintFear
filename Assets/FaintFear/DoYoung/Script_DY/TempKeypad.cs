using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 임시 키패드 상호작용 오브젝트
    /// 키패드 해제 → 문 자동 개방 연출 → 지하실 이동을 관리한다
    /// </summary>
    public class TempKeypad : Interactive, IActionProvider
    {
        #region Variables

        [Header("Scene")]
        [SerializeField] private string targetSceneName;            // 이동할 씬 이름
        public SceneFader sceneFader;

        [Header("Door")]
        [SerializeField] private Transform doorPivot;               // 회전할 문 피벗
        [SerializeField] private float openedAngle = -90f;          // 문이 완전히 열린 각도
        [SerializeField] private float openSpeed = 90f;             // 문 회전 속도

        [Header("Sequence")]
        [SerializeField] private SequenceTextManager sequenceText;  // 시퀀스 메시지 출력

        private bool isOpened = false;        // 문이 완전히 개방되었는지 여부
        private bool isOpening = false;       // 개방 연출 진행 중 여부

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            if (isOpening) return;
            // 만약 [개방 연출 중이라면] [상호작용을 차단한다]

            if (!isOpened)
            {
                StartCoroutine(OpenBasementRoutine());
                return;
                // 만약 [아직 개방되지 않았다면] [개방 연출을 시작하고 종료한다]
            }

            MoveToScene();
            // 만약 [이미 개방 완료 상태라면] [씬 이동을 요청한다]
        }

        // 지하실 개방 연출 전체 흐름
        private IEnumerator OpenBasementRoutine()
        {
            isOpening = true;
            // 개방 연출 시작 → 상호작용 차단

            if (sequenceText != null)
                sequenceText.ShowMessage("지하실이 개방됐다.");
            // 지하실 개방 시퀀스를 출력한다

            yield return new WaitForSeconds(0.5f);
            // 메시지 인지 시간을 잠시 확보한다

            yield return StartCoroutine(OpenDoorRoutine());
            // 문 개방 연출이 끝날 때까지 대기한다

            isOpened = true;
            isOpening = false;
            // 개방 완료 상태로 전환하고 상호작용을 다시 허용한다
        }

        // 문을 자동으로 여는 연출
        private IEnumerator OpenDoorRoutine()
        {
            if (doorPivot == null)
                yield break;
            // 만약 [문 피벗이 없다면] [연출을 수행하지 않는다]

            float currentAngle = doorPivot.localEulerAngles.y;
            float targetAngle = openedAngle;

            float t = 0f;
            float duration = Mathf.Abs(targetAngle - currentAngle) / openSpeed;

            while (t < duration)
            {
                t += Time.deltaTime;

                float angle = Mathf.Lerp(currentAngle, targetAngle, t / duration);
                doorPivot.localEulerAngles = new Vector3(0f, angle, 0f);

                yield return null;
            }

            doorPivot.localEulerAngles = new Vector3(0f, targetAngle, 0f);
            // 문이 90도로 완전히 열린 상태를 보장한다
        }

        // SceneLoadManager에 씬 이동을 요청한다
        private void MoveToScene()
        {
            if (string.IsNullOrEmpty(targetSceneName)) return;
            // 만약 [씬 이름이 비어 있다면] [이동하지 않는다]
            sceneFader.FadeTo(targetSceneName);
            //씬 이동
        }

        #endregion


        #region Property

        // Action UI에 표시할 문구를 제공한다
        public string GetActionText()
        {
            if (isOpening)
                return string.Empty;
            // 만약 [연출 중이라면] [액션 UI를 숨긴다]

            return isOpened ? "이동하기" : "사용하기";
            // 상태에 따라 액션 문구를 변경한다
        }

        #endregion
    }
}
