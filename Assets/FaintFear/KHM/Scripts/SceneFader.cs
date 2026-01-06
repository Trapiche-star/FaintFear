using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace FaintFear
{
    /// <summary>
    /// 씬 페이드인, 페이드 아웃 기능
    /// 페이드 아웃 후 씬 이동 + 스폰 위치 설정
    /// </summary>
    public class SceneFader : MonoBehaviour
    {
        #region Variables
        public Image img;
        public AnimationCurve curve;

        // ⭐ 씬 전환 시 사용할 스폰 정보 저장
        public static Vector3? targetSpawnPosition = null;
        public static Quaternion? targetSpawnRotation = null;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //페이더 이미지를 검정색으로 시작 - 씬을 시작하면 무조건 암전
            img.color = new Color(0f, 0f, 0f, 1);
        }
        #endregion

        #region Custom Method
        //페이드인 시작
        public void FadeStart(float delayTime = 0f)
        {
            StartCoroutine(FadeIn(delayTime));
        }

        //페이드인: 1초동안 이미지 a: 1 -> 0
        IEnumerator FadeIn(float delayTime)
        {
            if (delayTime >= 0f)
            {
                yield return new WaitForSeconds(delayTime);
            }
            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                float a = curve.Evaluate(t);
                img.color = new Color(0f, 0f, 0f, a);
                yield return 0;
            }
        }

        // ⭐ 기존 메서드 (스폰 위치 없음)
        public void FadeTo(string sceneName)
        {
            StartCoroutine(FadeOut(sceneName));
        }

        //Transform으로 스폰 위치 지정
        public void FadeTo(string sceneName, Transform spawnPoint)
        {
            if (spawnPoint != null)
            {
                targetSpawnPosition = spawnPoint.position;
                targetSpawnRotation = spawnPoint.rotation;
            }
            StartCoroutine(FadeOut(sceneName));
        }

        //페이드 아웃 : 1초동안 이미지 a: 0 -> 1
        IEnumerator FadeOut(string sceneName)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                float a = curve.Evaluate(t);
                img.color = new Color(0f, 0f, 0f, a);
                yield return 0;
            }

            //페이드 아웃 완료 후 다음씬으로 이동
            if (sceneName != string.Empty)
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        IEnumerator FadeOut(int buildIndex)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                float a = curve.Evaluate(t);
                img.color = new Color(0f, 0f, 0f, a);
                yield return 0;
            }

            //페이드 아웃 완료 후 다음씬으로 이동
            if (buildIndex >= 0)
            {
                SceneManager.LoadScene(buildIndex);
            }
        }

        /// <summary>
        /// ⭐ 씬 로드 후 플레이어를 스폰 위치로 이동
        /// GameManager나 씬 시작 스크립트에서 호출
        /// </summary>
        public static void ApplySpawnPosition()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("[SceneFader] Player not found!");
                return;
            }

            // ⭐ 방법 1: 저장된 Position/Rotation 사용
            if (targetSpawnPosition.HasValue)
            {
                CharacterController cc = player.GetComponent<CharacterController>();

                if (cc != null)
                {
                    cc.enabled = false;
                }

                player.transform.position = targetSpawnPosition.Value;

                if (targetSpawnRotation.HasValue)
                {
                    player.transform.rotation = targetSpawnRotation.Value;
                }

                if (cc != null)
                {
                    cc.enabled = true;
                }

                // 사용 후 초기화
                targetSpawnPosition = null;
                targetSpawnRotation = null;

                Debug.Log("[SceneFader] Player spawned at custom position");
                return;
            }
        }
        #endregion
    }
}