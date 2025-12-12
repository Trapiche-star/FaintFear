using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// HUD 매니저
    /// 페이드 및 대사 출력 중에는 플레이어의 이동/회전을 일시적으로 비활성화함
    /// </summary>
    public class HUDManager_01 : MonoBehaviour
    {
        // 텍스트 매니저 참조
        public SequenceTextManager textManager;

        // 대사 유지 시간 (필요 시 인스펙터에서 조정 가능)
        [SerializeField] private float dialogueHoldTime = 3f;

        void Start()
        {
            // 페이드용 이미지 및 페이더 참조
            Transform targetObj = transform.GetChild(1);
            Image targetImage = targetObj.GetChild(0).GetComponent<Image>();
            SceneFader fader = targetObj.GetComponent<SceneFader>();

            // 페이드 이미지 알파값 초기화 (완전 불투명)
            Color tempColor = targetImage.color;
            tempColor.a = 1f;
            targetImage.color = tempColor;

            // 텍스트 매니저 자동 연결 (없을 경우)
            if (textManager == null)
                textManager = transform.GetChild(0).GetComponent<SequenceTextManager>();

            // 플레이어 탐색
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerMove playerMove = null;
            if (player != null)
                playerMove = player.GetComponent<PlayerMove>();

            // 페이드 및 대사 중 제어 잠금
            if (playerMove != null)
                playerMove.enabled = false;

            // 페이드 아웃 시작
            fader.FadeOutToZero(() =>
            {
                // 페이드 완료 후 대사 출력 시작
                if (textManager != null)
                {
                    textManager.ShowMessage("폐병원을 순찰하다 침입자들을 쫓아 들어왔는데… " +
                        "배터리가 꺼져버렸군.");
                }

                // 코루틴으로 대사 유지 시간만큼 기다렸다가 제어 복귀
                StartCoroutine(RestoreControlAfterDelay(playerMove));
            });
        }

        // 일정 시간 후 플레이어 제어를 복원하는 코루틴
        private IEnumerator RestoreControlAfterDelay(PlayerMove playerMove)
        {
            // 지정된 시간 동안 대사 유지
            yield return new WaitForSeconds(dialogueHoldTime);

            // 플레이어 제어 복원
            if (playerMove != null)
                playerMove.enabled = true;
        }

        void Update()
        {
            // 현재 사용하지 않음 (추후 HUD 갱신용)
        }
    }
}
