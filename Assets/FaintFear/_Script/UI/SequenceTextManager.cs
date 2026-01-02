using UnityEngine;
using System.Collections;
using TMPro;

namespace FaintFear
{
    /// <summary>
    /// HUD 텍스트 출력과 문장 시퀀스를 관리하는 매니저
    /// </summary>
    public class SequenceTextManager : MonoBehaviour
    {
        #region Variables

        [SerializeField] private TextMeshProUGUI targetText; // HUD에 표시되는 텍스트 UI

        private Coroutine currentTimer;                      // 단일 메시지 자동 종료 타이머

        #endregion


        #region Unity Event Method

        private void Start()
        {
            // 만약 텍스트 참조가 없다면 자식에서 자동 탐색한다
            if (targetText == null)
                targetText = GetComponentInChildren<TextMeshProUGUI>();

            // 시작 시 텍스트는 비활성 상태로 둔다
            targetText.gameObject.SetActive(false);
        }

        #endregion


        #region Custom Method

        // 단일 문장을 즉시 출력하고 일정 시간 후 자동으로 숨긴다
        public void ShowMessage(string message, float duration = 3f)
        {
            // 텍스트가 없으면 출력할 수 없으므로 종료한다
            if (targetText == null) return;

            // 이전 자동 종료 타이머가 있다면 중단한다
            if (currentTimer != null)
                StopCoroutine(currentTimer);

            // 텍스트를 갱신하고 활성화한다
            targetText.text = message;
            targetText.gameObject.SetActive(true);

            // 지정된 시간 후 숨김 처리를 예약한다
            currentTimer = StartCoroutine(DisableTimer(duration));
        }

        // 여러 문장을 순서대로 출력하는 시퀀스를 실행한다
        public IEnumerator ShowDialogueSequence(string[] lines, float holdTime)
        {
            // 시퀀스 시작 시 단일 메시지용 타이머를 완전히 중단한다
            if (currentTimer != null)
            {
                StopCoroutine(currentTimer);
                currentTimer = null;
            }

            // 전달받은 모든 문장을 순서대로 출력한다
            foreach (string line in lines)
            {
                targetText.text = line;
                targetText.gameObject.SetActive(true);

                // 지정된 유지 시간만큼 대기한다
                yield return new WaitForSeconds(holdTime);
            }

            // 시퀀스 종료 후에는 텍스트를 숨기지 않는다
            // 이후 출력은 다음 메시지 호출부에 맡긴다
        }

        // 조건이 충족될 때까지 유지되는 메시지를 출력한다
        public void ShowPersistentMessage(string message)
        {
            // 텍스트가 없으면 출력할 수 없으므로 종료한다
            if (targetText == null) return;

            // 자동 종료 타이머가 있다면 중단한다
            if (currentTimer != null)
            {
                StopCoroutine(currentTimer);
                currentTimer = null;
            }

            // 텍스트를 갱신하고 활성화한다
            targetText.text = message;
            targetText.gameObject.SetActive(true);
        }

        // 텍스트를 즉시 숨긴다
        public void Hide()
        {
            // 텍스트가 없으면 처리할 수 없으므로 종료한다
            if (targetText == null) return;

            // 실행 중인 타이머가 있다면 중단한다
            if (currentTimer != null)
            {
                StopCoroutine(currentTimer);
                currentTimer = null;
            }

            // 텍스트 오브젝트를 비활성화한다
            targetText.gameObject.SetActive(false);
        }

        // 일정 시간이 지나면 텍스트를 자동으로 숨긴다
        private IEnumerator DisableTimer(float time)
        {
            // 지정된 시간만큼 대기한다
            yield return new WaitForSeconds(time);

            // 시간이 경과했으므로 텍스트를 숨긴다
            Hide();
        }

        #endregion
    }
}
