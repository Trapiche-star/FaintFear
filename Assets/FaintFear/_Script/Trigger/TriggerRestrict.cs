using TMPro;
using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 플레이어가 특정 트리거 영역 밖으로 나가지 못하게 제한하는 클래스
    /// </summary>    
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerRestrict : MonoBehaviour
    {
        public Transform player;                        // 플레이어 트랜스폼 참조
        [SerializeField]
        private float pushBackOffset = 0.15f;            // 밀려나는 거리 보정값
        public SequenceTextManager sequenceText;         // 시퀀스 텍스트 출력 관리자

        private string dialogueLine = "너무 어두워서 손전등 없이는 더 이상 나아갈 수 없다."; // 경고 문구

        private BoxCollider boxCollider;                 // 영역을 나타내는 박스 콜라이더
        private bool restrictionActive = true;           // 이동 제한이 활성화되어 있는지 여부

        void Start()
        {
            // BoxCollider 설정 초기화
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;                // 감지용으로 설정 (충돌 아님)
        }

        void LateUpdate()
        {
            // 이동 제한이 비활성화되어 있으면 종료
            if (!restrictionActive) return;

            // 플레이어가 없으면 종료
            if (player == null) return;

            // 트리거의 월드 기준 중심점 계산
            Vector3 worldCenter = transform.TransformPoint(boxCollider.center);
            // 콜라이더의 경계 범위 계산
            Bounds bounds = new Bounds(worldCenter, boxCollider.size);

            Vector3 playerPos = player.position;

            // 플레이어가 콜라이더 영역 밖에 있을 경우
            if (!bounds.Contains(playerPos))
            {
                // 경고 텍스트 표시
                sequenceText.gameObject.SetActive(true);
                sequenceText.ShowMessage(dialogueLine);

                // 플레이어를 가장 가까운 영역 경계 지점으로 되돌림
                Vector3 closestPoint = bounds.ClosestPoint(playerPos);
                Vector3 direction = (closestPoint - worldCenter).normalized;
                Vector3 correctedPosition = closestPoint - direction * pushBackOffset;

                // 플레이어 위치 수정
                player.position = correctedPosition;
            }
        }

        // 에디터에서 영역 표시용 (선택 시)
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 0.6f, 1f, 0.3f);    // 연한 파란색 반투명 박스
            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(col.center, col.size);       // 콜라이더 영역 표시
            }
        }

        // 외부에서 이동 제한 활성화 상태를 변경할 수 있게 하는 메서드
        public void SetRestriction(bool active)
        {
            restrictionActive = active;
        }
    }
}
