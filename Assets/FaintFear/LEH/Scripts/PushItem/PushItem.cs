using UnityEngine;
using System.Collections;

namespace FaintFear
{
    public class PushItem : Interactive, IActionProvider, ISaveableWorldObject
    {
        [Header("Save")]
        [SerializeField] private string uniqueId;

        [Header("Move Settings")]
        [SerializeField] private Transform movePosition;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private SequenceTextManager sequenceText;

        [Header("Sound IDs")]
        [SerializeField] private string startPushSFX = "SFX_Push_Start";
        [SerializeField] private string completePushSFX = "SFX_Push_Complete";

        [Header("Push Settings")]
        [SerializeField] private float chargeSpeed = 0.5f;
        [SerializeField] private float drainSpeed = 1f;

        private bool isCleared = false;
        private bool isPushing = false;
        private float currentPushProgress = 0f;

        private PlayerMove playerMove;
        public PushGaugeUI gaugeUI;

        private bool isInRange = false;

        // ===================== Save Interface =====================
        public string GetID() => uniqueId;

        public void Save(ref SaveData data)
        {
            if (!isCleared) return;

            // 중복 제거
            data.movedObjects.RemoveAll(x => x.id == uniqueId);

            data.movedObjects.Add(new MovedObjectData
            {
                id = uniqueId,
                position = transform.position
            });
        }
        public void Load(SaveData data)
        {
            var saved = data.movedObjects.Find(x => x.id == uniqueId);

            if (saved == null)
            {
                Debug.Log($"[PushItem] {uniqueId} Load - movedObjects에 없음, 초기 위치 유지");
                return;
            }

            Debug.Log($"[PushItem] {uniqueId} Load - 저장된 위치로 이동 시도");
            Debug.Log($"  현재 위치: {transform.position}");
            Debug.Log($"  목표 위치: {saved.position}");

            transform.position = saved.position;
            isCleared = true;
            DisablePushing();

            Debug.Log($"[PushItem] {uniqueId} Load - 이동 완료, isCleared = true");
            Debug.Log($"  최종 위치: {transform.position}");
        }

        // ===================== Unity =====================

        void Start()
        {
            if (movePosition == null)
            {
                Debug.LogError($"[PushItem] {name}: movePosition이 설정되지 않았습니다!");
            }
        }

        void Update()
        {
            if (isCleared || !isInRange) return;

            if (isPushing)
            {
                currentPushProgress += chargeSpeed * Time.deltaTime;
                currentPushProgress = Mathf.Clamp01(currentPushProgress);
                gaugeUI?.UpdateGauge(currentPushProgress);

                if (currentPushProgress >= 1f)
                    CompletePush();
            }
            else if (currentPushProgress > 0f)
            {
                currentPushProgress -= drainSpeed * Time.deltaTime;
                currentPushProgress = Mathf.Max(0f, currentPushProgress);
                gaugeUI?.UpdateGauge(currentPushProgress);

                if (currentPushProgress <= 0f)
                    gaugeUI?.HideGauge();
            }
        }

        public override void Interaction()
        {
            if (isCleared) return;
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
                    playerMove = player.GetComponent<PlayerMove>();
            }

            if (playerMove == null)
            {
                Debug.LogError("[PushItem] PlayerMove를 찾을 수 없습니다!");
                return;
            }

            playerMove.OnPushEvent += OnPushInput;
            sequenceText.ShowPersistentMessage("안으로 들어가려면 이 상자를 치워야 할 것 같다.");
        }

        public void DisablePushing()
        {
            isInRange = false;
            isPushing = false;
            currentPushProgress = 0f;

            if (playerMove != null)
                playerMove.OnPushEvent -= OnPushInput;

            gaugeUI?.HideGauge();
            sequenceText.Hide();
        }

        void OnPushInput(bool isPressed)
        {
            if (isCleared || !isInRange) return;

            isPushing = isPressed;

            if (isPressed)
            {
                gaugeUI?.ShowGauge();
                sequenceText.Hide();
                SoundManager.Instance?.PlaySFX(startPushSFX);
            }
        }

        void CompletePush()
        {
            isPushing = false;
            isCleared = true;
            DisablePushing();

            // ⭐ 런타임 이동 상태 기록
            RuntimeStateManager.RecordMovedObject(uniqueId, movePosition.position);

            SoundManager.Instance?.PlaySFX(completePushSFX);
            StartCoroutine(MoveToPosition());
        }
        IEnumerator MoveToPosition()
        {
            float originalY = transform.position.y;
            Vector3 targetPos = movePosition.position;
            targetPos.y = originalY;

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                Vector3 newPos = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
                newPos.y = originalY;
                transform.position = newPos;
                yield return null;
            }

            transform.position = targetPos;
        }

        public string GetActionText()
        {
            if (isCleared || isPushing) return "";
            return "[V] 꾹 눌러서 상자 치우기";
        }

        void OnDestroy()
        {
            if (playerMove != null)
                playerMove.OnPushEvent -= OnPushInput;
        }
    }
}