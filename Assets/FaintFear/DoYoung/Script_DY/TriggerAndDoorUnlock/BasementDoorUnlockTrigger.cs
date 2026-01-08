using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 지하실 도어 해제 상태를 전역 매니저에 기록하는 트리거
    /// </summary>
    public class BasementDoorUnlockTrigger : MonoBehaviour
    {
        #region Custom Method

        // 지하실 도어를 해제 상태로 기록한다
        public void TriggerUnlock()
        {
            if (BasementDoorManager.Instance == null) return;
            // 만약 매니저 인스턴스가 없다면 이 메서드에서는 더 이상 처리하지 않는다

            if (BasementDoorManager.Instance.IsBasementDoorUnlocked) return;
            // 만약 이미 해제된 상태라면 이 메서드에서는 더 이상 처리하지 않는다

            BasementDoorManager.Instance.UnlockBasementDoor();
            // 지하실 도어를 해제 상태로 전역에 기록한다
        }

        #endregion
    }
}
