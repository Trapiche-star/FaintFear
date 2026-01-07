using UnityEngine;
using UnityEngine.SceneManagement;

namespace FaintFear
{
    /// <summary>
    /// 엔딩 A 전용 트리거
    /// 호출되면 지정된 엔딩 씬으로 이동한다
    /// </summary>
    public class AEndingTrigger : MonoBehaviour
    {
        #region Variables

        [SerializeField] private string endingSceneName; // 이동할 엔딩 A 씬 이름

        //+
        [Header("BGM Settings")]
        [SerializeField] private string finalBGMName = "BGM_Final"; // + 재생할 상시 BGM 이름

        #endregion


        #region Custom Method

        // 엔딩 A 씬으로 이동한다
        public void ExecuteEnding()
        {
            //+ 트리거 이후 BGM_Final 재생
            if (!string.IsNullOrEmpty(finalBGMName) && SoundManager.Instance != null)
            {
                //IdleBGMManager가 있으면 잠시 비활성화
                IdleBGMManager[] idleManagers = Object.FindObjectsByType<IdleBGMManager>(FindObjectsSortMode.None);
                IdleBGMManager idleManager = idleManagers.Length > 0 ? idleManagers[0] : null;

                if (idleManager != null)
                {
                    idleManager.gameObject.SetActive(false); // Idle BGM 간섭 방지
                }

                //SoundManager를 통해 BGM 재생
                SoundManager.Instance.PlayBGM(finalBGMName);
            }

            // 1. 씬 이동
            if (!string.IsNullOrEmpty(endingSceneName))
            {
                SceneManager.LoadScene(endingSceneName);
                // + SoundManager는 DontDestroyOnLoad이므로 BGM은 유지됨
            }
        }
        /*
        if (string.IsNullOrEmpty(endingSceneName))
            return; // 만약 [엔딩 씬 이름이 지정되지 않았다면] [씬 이동을 실행하지 않는다]

        SceneManager.LoadScene(endingSceneName);
        // 지정된 엔딩 씬으로 이동한다 */
    }

        #endregion
    }

