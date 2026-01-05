using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 엔딩 조건 판별 매니저
    /// 파워박스 슬롯에 실제로 활성화된 레버 상태를 기준으로 엔딩 가능 여부를 판단한다
    /// </summary>
    public class EndingManager : MonoBehaviour
    {
        #region Variables

        // 레버 활성 상태 (0:빨강, 1:노랑, 2:검정, 3:파랑)
        private bool[] activatedLevers = new bool[4];

        #endregion


        #region Custom Method

        // 슬롯에서 레버 활성화를 전달받는다
        public void SetLeverActivated(int leverIndex)
        {
            if (leverIndex < 0 || leverIndex >= activatedLevers.Length)
                return; // 만약 [잘못된 인덱스라면] [처리하지 않는다]

            activatedLevers[leverIndex] = true;
            // 해당 레버를 활성 상태로 기록한다
        }

        // 엔딩 A 가능 여부를 판단한다
        public bool CanEnterEndingA()
        {
            bool allActivated =
                activatedLevers[0] &&
                activatedLevers[1] &&
                activatedLevers[2] &&
                activatedLevers[3];

            if (allActivated)
                return false; // 만약 [4개 레버가 모두 활성화되었다면] [엔딩 A는 차단된다]

            return activatedLevers[0];
            // 빨강 레버가 활성화되어 있다면 엔딩 A 가능
        }

        // 엔딩 B 가능 여부를 판단한다
        public bool CanEnterEndingB()
        {
            return
                activatedLevers[0] &&
                activatedLevers[1] &&
                activatedLevers[2] &&
                activatedLevers[3];
            // 4개 레버가 모두 활성화된 경우에만 엔딩 B 가능
        }

        #endregion
    }
}
