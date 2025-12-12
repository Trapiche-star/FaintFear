
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 플레이어가 트리거 영역 밖으로 나가지 못하도록 제한한다.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerRestrict : MonoBehaviour
    {
        public Transform player;
        public float pushBackOffset = 0.15f;

        private BoxCollider boxCollider;
        private bool restrictionActive = true;

        void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        void LateUpdate()
        {
            if (!restrictionActive) return;

            if (player == null) return;

            Vector3 worldCenter = transform.TransformPoint(boxCollider.center);
            Bounds bounds = new Bounds(worldCenter, boxCollider.size);

            Vector3 playerPos = player.position;

            if (!bounds.Contains(playerPos))
            {
                Vector3 closestPoint = bounds.ClosestPoint(playerPos);
                Vector3 direction = (closestPoint - worldCenter).normalized;
                Vector3 correctedPosition = closestPoint - direction * pushBackOffset;

                player.position = correctedPosition;
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

        // 외부 스크립트에서 이동 제한 On/Off 제어
        public void SetRestriction(bool active)
        {
            restrictionActive = active;
        }
    }
}
