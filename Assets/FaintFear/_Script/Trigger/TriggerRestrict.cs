using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 플레이어가 특정 트리거 영역 밖으로 나가지 못하게 제한하는 클래스
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerRestrict : MonoBehaviour
    {
        #region Variables

        public Transform player;                        // 플레이어 트랜스폼 참조

        [SerializeField]
        private float playerRadius = 0.3f;              // 플레이어 캡슐 반경

        [SerializeField]
        private float pushBackOffset = 0.15f;           // 밀려나는 거리 보정값

        public SequenceTextManager sequenceText;         // 경고 텍스트 출력 관리자

        private string dialogueLine =
            "너무 어두워서 손전등 없이는 더 이상 나아갈 수 없다."; // 경고 문구

        private BoxCollider boxCollider;                 // 영역을 나타내는 박스 콜라이더
        private bool restrictionActive = true;           // 이동 제한 활성 여부

        private bool warningShown = false;               // 경고 메시지 출력 여부

        #endregion


        #region Unity Event Method
        private void Awake()
        {
            if (GameManager.TutorialCompleted)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // BoxCollider를 감지용 트리거로 초기화한다
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        private void LateUpdate()
        {
            // 이동 제한이 비활성화되어 있으면 더 이상 처리하지 않는다
            if (!restrictionActive) return;

            // 플레이어 참조가 없으면 처리할 수 없으므로 종료한다
            if (player == null) return;

            // 트리거의 월드 기준 중심점을 계산한다
            Vector3 worldCenter = transform.TransformPoint(boxCollider.center);

            // BoxCollider 크기를 기준으로 Bounds를 생성한다
            Bounds bounds = new Bounds(worldCenter, boxCollider.size);

            Vector3 playerPos = player.position;

            // 만약 플레이어가 제한 영역 밖에 있다면
            if (!bounds.Contains(playerPos))
            {
                // 아직 경고 메시지를 출력하지 않았다면 한 번만 출력한다
                if (!warningShown)
                {
                    sequenceText.ShowMessage(dialogueLine); // 출력 요청만 수행한다
                    warningShown = true;                    // 중복 출력 방지
                }

                // 플레이어 위치에서 가장 가까운 영역 경계 지점을 계산한다
                Vector3 closestPoint = bounds.ClosestPoint(playerPos);

                // 영역 중심 방향으로 밀어내기 위한 방향 벡터를 계산한다
                Vector3 dirToCenter = (worldCenter - playerPos).normalized;

                // 플레이어 반경과 보정값을 고려하여 안쪽으로 위치를 보정한다
                Vector3 correctedPosition =
                    closestPoint + dirToCenter * (playerRadius + pushBackOffset);

                // 플레이어 위치를 강제로 영역 안쪽으로 되돌린다
                player.position = correctedPosition;
            }
            else
            {
                // 플레이어가 다시 영역 안으로 들어왔을 경우
                warningShown = false; // 다음 이탈 시 다시 출력 가능하도록 초기화
            }
        }

        #endregion


        #region Custom Method

        // 외부 이벤트에서 이동 제한 활성 상태를 제어한다
        public void SetRestriction(bool active)
        {
            restrictionActive = active;
        }

        #endregion


#if UNITY_EDITOR

        // 에디터에서 제한 영역을 시각적으로 표시한다 (선택 시)
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 0.6f, 1f, 0.3f); // 연한 파란색 반투명 박스
            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(col.center, col.size);   // 콜라이더 영역 표시
            }
        }

#endif
    }
}
