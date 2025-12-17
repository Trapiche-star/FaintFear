using UnityEngine;
using System.Collections;
namespace FaintFear
{
    public class GameHUDManager : MonoBehaviour
    {

        [Header("UI References")]
        [SerializeField] private GameObject sequenceUI;
        [SerializeField] private GameObject pushGaugeUI;
        [SerializeField] private SceneFader sceneFader;
        [SerializeField] private SequenceTextManager textManager;

        void Start()
        {
            //시작 시 파워 게이지 숨김
            if (pushGaugeUI != null)
                pushGaugeUI.SetActive(false);

            //시퀀스 UI 보이기
            if (sequenceUI != null)
                sequenceUI.SetActive(true);

            //플레이어 이동 잠금
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerMove playerMove = player != null ? player.GetComponent<PlayerMove>() : null;
            if (playerMove != null)
                playerMove.enabled = false;

            //페이드 아웃
            if (sceneFader != null)
            {
                sceneFader.FadeOutToZero(() =>
                {
                    if (textManager != null)
                    {
                        textManager.ShowMessage(
                            "폐병원을 순찰하다 침입자들을 쫓아 들어왔는데… 배터리가 꺼져버렸군."
                        );
                    }

                    StartCoroutine(RestoreControlAfterDelay(playerMove));
                });
            }
        }

        IEnumerator RestoreControlAfterDelay(PlayerMove playerMove)
        {
            yield return new WaitForSeconds(3f);

            //시퀀스 종료
            if (sequenceUI != null)
                sequenceUI.SetActive(false);

            //플레이어 조작 복귀
            if (playerMove != null)
                playerMove.enabled = true;

            //PushGaugeUI는 여기서 켜지지 않음
        }
    }
}