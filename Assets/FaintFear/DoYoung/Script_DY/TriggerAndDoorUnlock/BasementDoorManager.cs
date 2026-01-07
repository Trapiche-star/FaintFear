using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 지하실 도어 해제 여부를 전역에서 단일 관리하는 매니저
    /// </summary>
    public class BasementDoorManager : MonoBehaviour
    {
        #region Variables

        public static BasementDoorManager Instance; // 전역 접근용 싱글톤 인스턴스

        private bool isBasementDoorUnlocked = false; // 지하실 도어가 해제되었는지 여부

        #endregion


        #region Unity Event Method

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);               // 만약 이미 인스턴스가 있다면 이 객체는 제거한다
                return;                            // 중복 생성을 방지하기 위해 더 이상 실행하지 않는다
            }

            Instance = this;                       // 전역에서 접근 가능한 인스턴스로 등록한다
            DontDestroyOnLoad(gameObject);         // 씬 이동 시에도 파괴되지 않도록 설정한다
        }

        #endregion


        #region Custom Method

        // 지하실 도어를 해제 상태로 설정한다
        public void UnlockBasementDoor()
        {
            if (isBasementDoorUnlocked) return;    // 만약 이미 해제된 상태라면 다시 처리하지 않는다

            isBasementDoorUnlocked = true;         // 지하실 도어를 해제 상태로 기록한다
        }

        #endregion


        #region Property

        public bool IsBasementDoorUnlocked => isBasementDoorUnlocked; // 외부에서 읽기 전용으로 상태를 조회한다

        #endregion
    }
}
