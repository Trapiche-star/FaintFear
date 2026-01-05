/*using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 플레이어가 생성될 때까지 대기한 뒤
    /// 지정된 스폰 포인트로 이동시키고
    /// 그 이후에만 페이드 알파를 서서히 제거한다
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        #region Variables

        [Header("Spawn")]
        [SerializeField] private Transform spawnPoint;
        // 플레이어 스폰 위치

        [Header("Fade")]
        [SerializeField] private SceneFader sceneFader;
        // 씬 시작 시 이미 검은 화면을 유지 중인 페이더

        [SerializeField] private float postSpawnDelay = 0.05f;
        // 스폰 직후 안정화 대기 시간

        #endregion


        #region Unity Event Method

        private void Start()
        {
            StartCoroutine(SpawnRoutine());
        }

        #endregion


        #region Custom Method

        private IEnumerator SpawnRoutine()
        {
            // 1. 플레이어가 생성될 때까지 대기
            GameObject player = null;
            yield return new WaitUntil(() =>
            {
                player = GameObject.FindGameObjectWithTag("Player");
                return player != null;
            });

            // 2. CharacterController가 있다면 비활성화 후 이동
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
                cc.enabled = false;

            // 3. 스폰 포인트로 이동
            if (spawnPoint != null)
            {
                player.transform.SetPositionAndRotation(
                    spawnPoint.position,
                    spawnPoint.rotation
                );
            }

            // 4. CharacterController 복구
            if (cc != null)
                cc.enabled = true;

            // 5. 스폰 안정화 대기 (충돌 / 지면 보정)
            yield return new WaitForSeconds(postSpawnDelay);

            // 6. 이제서야 검은 화면 알파 제거
            if (sceneFader != null)
                sceneFader.FadeOutToZero();
        }

        #endregion
    }
}
*/