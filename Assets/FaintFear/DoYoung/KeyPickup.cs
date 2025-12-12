using UnityEngine;
using UnityEngine.InputSystem;

namespace FaintFear
{
    /// <summary>
    /// 플레이어의 열쇠 획득 처리 스크립트
    /// - 'Key' 태그를 가진 오브젝트와의 트리거 감지
    /// - E키 입력으로 열쇠를 획득
    /// - 액션 UI(Press [E]) 표시 및 숨김
    /// </summary>
    public class KeyPickup : MonoBehaviour
    {
        [Header("열쇠 상태")]
        public bool hasKey = false; // true면 이미 열쇠를 주운 상태

        // 내부 동작용 변수
        private bool isNearKey = false;      // 플레이어가 열쇠 근처에 있는지 여부
        private GameObject targetKeyObj;     // 현재 감지된 열쇠 오브젝트 참조

        [Header("UI 관련")]
        private ActionUI actionUI;            // [E] 표시용 UI 참조

        private void Start()
        {
            // ActionUI 자동 탐색 (씬에 하나만 존재한다고 가정)
            actionUI = FindFirstObjectByType<ActionUI>();
        }

        /// <summary>
        /// E키 입력을 감지하는 Input System 이벤트
        /// (PlayerInput의 Interact 액션과 연결됨)
        /// </summary>
        public void OnInteract(InputAction.CallbackContext context)
        {
            // E키가 눌린 순간(performed 상태)만 처리
            if (!context.performed) return;

            // 플레이어가 열쇠 근처에 있고, 아직 열쇠를 안 주웠을 경우에만 실행
            if (isNearKey && targetKeyObj != null && !hasKey)
            {
                PickupKey(); // 열쇠 획득 처리 호출
            }
        }

        /// <summary>
        /// 플레이어가 'Key' 태그의 오브젝트 트리거 범위에 진입했을 때 호출
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // 열쇠를 아직 주우지 않았고, 접근한 오브젝트가 Key 태그일 때
            if (other.CompareTag("Key") && !hasKey)
            {
                isNearKey = true;
                targetKeyObj = other.gameObject;

                // 액션 UI 표시 (Press [E] 열쇠 줍기)
                actionUI?.ShowAction("열쇠 줍기");
            }
        }

        /// <summary>
        /// 플레이어가 'Key' 오브젝트 범위를 벗어났을 때 호출
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Key"))
            {
                isNearKey = false;
                targetKeyObj = null;

                // 액션 UI 숨기기
                actionUI?.HideAction();
            }
        }

        /// <summary>
        /// 실제 열쇠 획득 처리 함수
        /// </summary>
        private void PickupKey()
        {
            hasKey = true; // 열쇠 보유 상태로 전환
            Debug.Log("열쇠를 획득했다!");

            // 시퀀스 텍스트 출력 (선택)
            var seqText = FindFirstObjectByType<SequenceTextManager>();
            if (seqText != null)
                seqText.ShowMessage("이걸로 저쪽 문을 열 수 있을지도 모른다.");

            // 액션 UI 숨김
            actionUI?.HideAction();

            // 실제 키 오브젝트를 씬에서 제거
            if (targetKeyObj != null)
                Destroy(targetKeyObj);

            // 상태 초기화
            isNearKey = false;
            targetKeyObj = null;
        }
    }
}
