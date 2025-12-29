using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 씬 시작 시 플레이어를 지정된 스폰 포인트로 이동시키고 페이드 연출을 수행한다
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        #region Variables

        [Header("Spawn")]
        [SerializeField] private Transform spawnPoint;
        // 플레이어가 스폰될 위치 (인스펙터에서 직접 지정)

        [Header("Fade")]
        [SerializeField] private SceneFader sceneFader;
        // 화면 페이드 연출 담당

        [Header("Sequence")]
        [SerializeField] private SequenceTextManager sequenceText;
        // 시퀀스 텍스트 출력 담당

        [SerializeField] private float delayAfterFade = 0.1f;
        // 페이드 완료 후 연출 안정화를 위한 대기 시간

        #endregion


        #region Unity Event Method

        // 씬이 시작되면 자동으로 스폰 처리를 시작한다
        private void Start()
        {
            StartCoroutine(SpawnRoutine());
        }

        #endregion


        #region Custom Method

        // 페이드 → 스폰 → 페이드 → 시퀀스 출력 흐름을 처리한다
        private IEnumerator SpawnRoutine()
        {
            bool fadeCompleted = false;

            if (sceneFader != null)
            {
                sceneFader.FadeInToOne(() => fadeCompleted = true);
                // 만약 [페이더가 존재한다면] [화면을 검게 만들고 완료 시점을 대기한다]

                yield return new WaitUntil(() => fadeCompleted);
                // 페이드 인이 끝날 때까지 대기한다
            }

            yield return new WaitForSeconds(delayAfterFade);
            // 페이드 종료 후 잠시 대기한다

            SpawnPlayerAtPoint();
            // 플레이어를 지정된 스폰 포인트 위치로 이동시킨다

            fadeCompleted = false;

            if (sceneFader != null)
            {
                sceneFader.FadeOutToZero(() => fadeCompleted = true);
                // 만약 [페이더가 존재한다면] [화면을 다시 밝힌다]

                yield return new WaitUntil(() => fadeCompleted);
                // 페이드 아웃이 끝날 때까지 대기한다
            }

            yield return new WaitForSeconds(delayAfterFade);
            // 페이드 종료 후 시퀀스 출력 타이밍을 맞춘다

            if (sequenceText != null)
                sequenceText.ShowMessage("지하실로 이동했다.");
            // 이동 완료 시퀀스 메시지를 출력한다
        }

        // 플레이어를 스폰 포인트 위치로 이동시킨다
        private void SpawnPlayerAtPoint()
        {
            if (spawnPoint == null)
                return;
            // 만약 [스폰 포인트가 지정되지 않았다면] [이동하지 않는다]

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;
            // 만약 [플레이어 오브젝트가 없다면] [처리를 중단한다]

            player.transform.SetPositionAndRotation(
                spawnPoint.position,
                spawnPoint.rotation
            );
            // 플레이어를 스폰 포인트 위치와 회전값으로 이동시킨다
        }

        #endregion
    }
}
