using UnityEngine;
using UnityEngine.SceneManagement;

namespace FaintFear
{
    /// <summary>
    /// 엔딩 B 전용 트리거
    /// 호출되면 지정된 엔딩 씬으로 이동한다
    /// </summary>
    public class BEndingTrigger : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] private string endingSceneName; // 이동할 엔딩 B 씬 이름

        #endregion


        #region Custom Method

        // 엔딩 B 씬으로 이동한다
        public void ExecuteEnding()
        {
            if (string.IsNullOrEmpty(endingSceneName))
                return; // 만약 [엔딩 씬 이름이 지정되지 않았다면] [씬 이동을 실행하지 않는다]

            SceneManager.LoadScene(endingSceneName);
            // 지정된 엔딩 씬으로 이동한다
        }

        #endregion
    }
}
