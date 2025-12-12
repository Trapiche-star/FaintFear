
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 플레이어가 트리거 영역 밖으로 나가지 못하도록 제한한다.
    /// 단, 손전등에 배터리가 충전되면 이동 제한이 해제된다.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerRestrict : MonoBehaviour
    {
        #region Variables

        [Header("플레이어 오브젝트 (CharacterController가 붙어 있음)")]
        public Transform player; // Player 오브젝트 연결

        [Header("플레이어의 손전등 (Flashlight 스크립트)")]
        public Flashlight flashlight; // Player 자식 오브젝트에 있는 Flashlight 연결

        [Header("트리거 경계 여유 거리 (플레이어를 안쪽으로 되돌릴 거리)")]
        public float pushBackOffset = 0.15f;

        private BoxCollider boxCollider;
        private bool restrictionActive = true; // true일 때 이동 제한 활성

        #endregion


        #region Unity Event Method

        private void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;

            if (player == null)
                Debug.LogWarning("TriggerRestrict: Player가 지정되지 않았습니다.");

            if (flashlight == null)
                Debug.LogWarning("TriggerRestrict: Flashlight 스크립트가 지정되지 않았습니다.");
        }

        private void LateUpdate()
        {
            if (player == null || boxCollider == null)
                return;

            // 손전등이 존재하고 배터리가 있으면 제한 해제
            if (flashlight != null && HasBattery())
            {
                if (restrictionActive)
                {
                    restrictionActive = false;
                    Debug.Log("TriggerRestrict: 손전등 배터리 감지됨, 이동 제한 해제");
                }
                return;
            }

            // 배터리 없으면 이동 제한 유지
            restrictionActive = true;

            // 트리거 경계 계산
            Vector3 worldCenter = transform.TransformPoint(boxCollider.center);
            Bounds bounds = new Bounds(worldCenter, boxCollider.size);

            Vector3 playerPos = player.position;

            // 플레이어가 경계 밖이라면 안쪽으로 되돌림
            if (restrictionActive && !bounds.Contains(playerPos))
            {
                Vector3 closestPoint = bounds.ClosestPoint(playerPos);
                Vector3 direction = (closestPoint - worldCenter).normalized;
                Vector3 correctedPosition = closestPoint - direction * pushBackOffset;

                player.position = correctedPosition;

                Debug.DrawLine(playerPos, correctedPosition, Color.red, 0.2f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 0.6f, 1f, 0.3f);
            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(col.center, col.size);
            }
        }

        #endregion


        #region Custom Method

        /// <summary>
        /// 손전등의 배터리 잔량이 남아 있는지 확인한다.
        /// </summary>
        private bool HasBattery()
        {
            var batteryField = typeof(Flashlight).GetField("currentBattery",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (batteryField != null)
            {
                float currentBattery = (float)batteryField.GetValue(flashlight);
                return currentBattery > 0f;
            }

            return false;
        }

        #endregion


        #region Property

        /// <summary>
        /// 외부에서 현재 제한 상태를 확인할 수 있도록 제공
        /// </summary>
        public bool IsRestricted => restrictionActive;

        #endregion
    }
}
