using UnityEngine;
using System.Collections;

namespace FaintFear
{
    public class PushItem : Interactive, IActionProvider
    {
        [Header("Move Settings")]
        [SerializeField] private Transform movePosition;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private SequenceTextManager sequenceText; 


        [Header("Push Settings")]
        [SerializeField] private float chargeSpeed = 0.5f;  // 1초에 증가하는 량 
        [SerializeField] private float drainSpeed = 1f;     // 1초에 감소하는 량 

        private bool isCleared = false;
        private bool isPushing = false;
        private float currentPushProgress = 0f;

        private PlayerMove playerMove;
        public PushGaugeUI gaugeUI;

        private bool isInRange = false;

        void Start()
        {
            if (movePosition == null)
            {
                Debug.LogError($"[PushItem] {name}: movePosition이 설정되지 않았습니다!");
            }
        }

        void Update()
        {
            if (isCleared) return;
            if (!isInRange) return;

            // 게이지 증가/감소 처리
            if (isPushing)
            {
                // ⭐ 1초에 chargeSpeed만큼 증가
                currentPushProgress += chargeSpeed * Time.deltaTime;
                currentPushProgress = Mathf.Clamp01(currentPushProgress);

                if (gaugeUI != null)
                {
                    gaugeUI.UpdateGauge(currentPushProgress);
                }

                if (currentPushProgress >= 1f)
                {
                    CompletePush();
                }
            }
            else
            {
                if (currentPushProgress > 0f)
                {
                    // ⭐ 1초에 drainSpeed만큼 감소
                    currentPushProgress -= drainSpeed * Time.deltaTime;
                    currentPushProgress = Mathf.Max(0f, currentPushProgress);

                    if (gaugeUI != null)
                    {
                        gaugeUI.UpdateGauge(currentPushProgress);
                    }

                    // 게이지가 0이 되면 UI 숨김
                    if (currentPushProgress <= 0f && gaugeUI != null)
                    {
                        gaugeUI.HideGauge();
                    }
                }
            }
        }

        public override void Interaction()
        {
            if (isCleared)
            {
                return;
            }

            EnablePushing();
        }

        public void EnablePushing()
        {
            if (isCleared || isInRange) return;

            isInRange = true;

            if (playerMove == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    playerMove = player.GetComponent<PlayerMove>();
                }
            }

            if (playerMove == null)
            {
                Debug.LogError("[PushItem] PlayerMove를 찾을 수 없습니다!");
                return;
            }

            playerMove.OnPushEvent += OnPushInput;

            sequenceText.ShowPersistentMessage("안으로 들어가려면 이 상자를 치워야 할 것 같다.");

            Debug.Log($"[PushItem] {name}: V키를 눌러서 밀 수 있습니다.");
        }

        public void DisablePushing()
        {
            if (!isInRange) return;

            isInRange = false;
            isPushing = false;
            currentPushProgress = 0f;

            if (playerMove != null)
            {
                playerMove.OnPushEvent -= OnPushInput;
            }

            if (gaugeUI != null)
            {
                gaugeUI.HideGauge();
            }

            sequenceText.Hide();
            Debug.Log($"[PushItem] {name}: 범위를 벗어났습니다.");
        }

        void OnPushInput(bool isPressed)
        {
            if (isCleared || !isInRange) return;

            isPushing = isPressed;

            if (isPressed)
            {
                if (gaugeUI != null)
                {
                    gaugeUI.ShowGauge();
                }

                sequenceText.Hide();
                Debug.Log($"[PushItem] V키 누름 - 게이지 충전 중");
            }
            else
            {
                Debug.Log($"[PushItem] V키 뗌 - 게이지 감소 중");
            }
        }

        void CompletePush()
        {
            Debug.Log($"[PushItem] {name}: 게이지 완료! 이동 시작");

            isPushing = false;
            isCleared = true;
            isInRange = false;

            if (playerMove != null)
            {
                playerMove.OnPushEvent -= OnPushInput;
            }

            if (gaugeUI != null)
            {
                gaugeUI.HideGauge();
            }

            StartCoroutine(MoveToPosition());
        }

        IEnumerator MoveToPosition()
        {
            if (movePosition == null)
            {
                Debug.LogError($"[PushItem] {name}: movePosition이 없습니다!");
                yield break;
            }

            // ⭐ 시작 Y값 저장
            float originalY = transform.position.y;
            Vector3 targetPos = movePosition.position;

            // ⭐ 목표 위치의 Y값도 현재 Y값으로 고정
            targetPos.y = originalY;

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                Vector3 newPos = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

                // ⭐ Y값 고정
                newPos.y = originalY;

                transform.position = newPos;

                yield return null;
            }

            // ⭐ 최종 위치도 Y값 고정
            targetPos.y = originalY;
            transform.position = targetPos;

            Debug.Log($"[PushItem] {name}: 이동 완료");
        }

        public string GetActionText()
        {
            if (isCleared || isPushing)
            {
                return "";
            }

            return "[V] 꾹 눌러서 상자 치우기";
        }

        void OnDestroy()
        {
            if (playerMove != null)
            {
                playerMove.OnPushEvent -= OnPushInput;
            }
        }
    }
}