using UnityEngine;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 사슬과 자물쇠로 봉인된 문을 제어하는 체인락 도어 컨트롤러
    /// 볼트 커터로 사슬을 제거한 이후에만 문을 열 수 있다.
    /// </summary>
    public class ChainLock : Interactive, IActionProvider
    {
        #region Variables

        [Header("Door")]
        [SerializeField] private Transform hinge;          // 문 회전을 담당하는 힌지 트랜스폼
        [SerializeField] private float openAngle = -90f;   // 문이 열릴 목표 각도
        [SerializeField] private float rotateDuration = 1f;// 문 회전에 걸리는 시간

        [Header("Chain")]
        [SerializeField] private GameObject chainRoot;     // 사슬·자물쇠 오브젝트 묶음 (자식)

        [Header("UI")]
        [SerializeField] private SequenceTextManager sequenceText; // 텍스트 출력과 시퀀스를 담당

        private bool isOpen = false;                        // 문 개방 상태
        private bool isMoving = false;                      // 문 회전 중 여부

        #endregion


        #region Custom Method

        // 플레이어 상호작용 처리
        public override void Interaction()
        {
            if (isMoving) return;
            // 만약 [문이 회전 중이라면] [중복 상호작용을 막기 위해 종료한다]

            if (IsLocked())
            {
                TryCutChain();
                return;
                // 만약 [체인이 남아 있다면] [문을 열지 않고 체인 처리만 시도한다]
            }

            ToggleDoor();
            // 체인이 제거된 상태라면 문 열기 또는 닫기를 수행한다
        }

        // 현재 문이 체인에 의해 잠겨 있는지 여부
        private bool IsLocked()
        {
            return chainRoot != null && chainRoot.activeSelf;
            // 사슬 오브젝트가 활성화되어 있으면 잠긴 상태로 판단한다
        }

        // 볼트 커터로 체인을 제거 시도
        private void TryCutChain()
        {
            if (PuzzleInventory.Instance == null ||
                !PuzzleInventory.Instance.HasBoltCutter)
            {
                ShowMessage("사슬과 자물쇠로 단단히 잠겨 있다.\n자를 것이 필요하다.");
                return;
                // 만약 [볼트 커터가 없다면] [힌트 메시지를 출력하고 종료한다]
            }

            chainRoot.SetActive(false);
            // 사슬과 자물쇠 오브젝트를 제거한다

            ShowMessage("볼트 커터로 사슬과 자물쇠를 끊어냈다.");
            // 체인 제거 성공 메시지를 출력한다
        }

        // 문 열기 / 닫기 토글
        private void ToggleDoor()
        {
            StartCoroutine(RotateDoor(isOpen ? 0f : openAngle));
            // 현재 문 상태에 따라 목표 각도를 결정하여 회전을 시작한다

            isOpen = !isOpen;
            // 문 개방 상태를 반전시킨다
        }

        // 문 회전 애니메이션 처리
        private IEnumerator RotateDoor(float targetAngle)
        {
            isMoving = true;
            // 문 회전 중 상태로 전환한다

            Quaternion startRot = hinge.localRotation;
            Quaternion targetRot = Quaternion.Euler(0f, targetAngle, 0f);

            float elapsed = 0f;
            while (elapsed < rotateDuration)
            {
                elapsed += Time.deltaTime;
                hinge.localRotation = Quaternion.Lerp(startRot, targetRot, elapsed / rotateDuration);
                yield return null;
                // 지정된 시간 동안 문을 부드럽게 회전시킨다
            }

            hinge.localRotation = targetRot;
            isMoving = false;
            // 회전 완료 후 상태를 복구한다
        }

        // HUD 메시지 출력
        private void ShowMessage(string message)
        {
            if (sequenceText == null) return;
            // 텍스트 매니저가 없으면 메시지를 출력하지 않는다

            sequenceText.ShowMessage(message);
            // 시퀀스 텍스트로 메시지를 출력한다
        }

        #endregion


        #region Property

        // Action UI에 표시될 문구 제공
        public string GetActionText()
        {
            return isOpen ? "문 닫기" : "문 열기";
            // 문 상태에 따라 액션 UI 텍스트를 반환한다
        }

        #endregion
    }
}
