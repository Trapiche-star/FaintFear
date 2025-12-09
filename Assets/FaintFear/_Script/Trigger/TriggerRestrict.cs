using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 플레이어가 트리거 영역(예: LightZone) 밖으로 나가지 못하도록 제한하는 스크립트
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerRestrict : MonoBehaviour
    {
        [Header("트리거 내부에 있어야 하는 대상 (Player 등)")]
        public Transform target; // 플레이어 Transform을 드래그해서 연결

        [Header("트리거의 경계 여유 거리 (안쪽으로 밀어넣는 거리)")]
        public float pushBackOffset = 0.2f; // 살짝 안쪽으로 되돌리기 위한 오프셋

        private BoxCollider boxCollider;

        void Start()
        {
            // BoxCollider 가져오기
            boxCollider = GetComponent<BoxCollider>();
            // 트리거로 설정되어 있어야 함
            boxCollider.isTrigger = true;
        }

        void Update()
        {
            if (target == null || boxCollider == null)
                return;

            // ──────────────────────────────────────────────
            // 트리거 범위를 벗어났는지 검사
            // BoxCollider의 중심과 사이즈를 이용해 AABB 검사
            // ──────────────────────────────────────────────
            Vector3 worldCenter = transform.TransformPoint(boxCollider.center);
            Vector3 halfSize = boxCollider.size * 0.5f;

            // 경계 계산 (BoxCollider는 로컬 기준이므로 TransformPoint로 변환)
            Bounds bounds = new Bounds(worldCenter, boxCollider.size);

            // 대상(플레이어) 위치
            Vector3 playerPos = target.position;

            // ──────────────────────────────────────────────
            // 플레이어가 범위 밖이면, 가장 가까운 점으로 이동시켜 되돌림
            // ──────────────────────────────────────────────
            if (!bounds.Contains(playerPos))
            {
                // BoxCollider 경계 안쪽의 가장 가까운 점 계산
                Vector3 closestPoint = bounds.ClosestPoint(playerPos);

                // 안쪽으로 살짝 밀어넣기 (옵션)
                Vector3 direction = (closestPoint - worldCenter).normalized;
                Vector3 correctedPosition = closestPoint - direction * pushBackOffset;

                // 플레이어 위치를 수정
                target.position = correctedPosition;

                // 디버그 표시 (선택 사항)
                Debug.DrawLine(playerPos, correctedPosition, Color.red, 0.2f);

                // 콘솔 로그 (디버그용)
                // Debug.Log("Player attempted to leave TriggerRestrict zone — corrected position.");
            }
        }

        // ──────────────────────────────────────────────
        // 에디터에서 트리거 영역 시각화 (디버그용)
        // ──────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 0.7f, 1f, 0.3f);
            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(col.center, col.size);
            }
        }
    }
}
