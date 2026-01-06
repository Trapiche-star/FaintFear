using UnityEngine;

namespace FaintFear
{
    public class EnemyAudio : MonoBehaviour
    {
        private bool isChasing;
        private float idleTimer;

        private void Awake()
        {
            ResetIdleTimer();
        }

        private void Update()
        {
            PlayIdleNoise();
        }

        /* =========================
         * 배회 중 랜덤 효과음
         * ========================= */
        private void PlayIdleNoise()
        {
            if (isChasing) return;

            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                SoundManager.Instance.PlaySFX(
                    Random.value > 0.5f ? "SFX_EnemyN_01" : "SFX_EnemyN_02"
                );

                ResetIdleTimer();
            }
        }

        private void ResetIdleTimer()
        {
            idleTimer = Random.Range(4f, 8f);
        }

        /* =========================
         * Enemy_Ex에서 호출
         * ========================= */

        public void OnChaseStart()
        {
            if (isChasing) return;

            isChasing = true;
            SoundManager.Instance.PlayBGM("BGM_Chase");
            SoundManager.Instance.PlaySFX("SFX_EnemyStart");
        }

        public void OnChaseEnd()
        {
            if (!isChasing) return;

            isChasing = false;
            SoundManager.Instance.PlayBGM("BGM_ChaseEnd");
        }

        public void OnAttack()
        {
            SoundManager.Instance.PlaySFX(
                Random.value > 0.5f ? "SFX_EnemyA_01" : "SFX_EnemyA_02"
            );
        }

        public void OnHitPlayer()
        {
            SoundManager.Instance.PlaySFX("SFX_Hurt");
        }
    }
}
