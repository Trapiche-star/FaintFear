using UnityEngine;
using System.Collections;

namespace FaintFear
{
    public class PushItem : Interactive
    {
        [Header("Move Target")]
        public Transform targetPoint;

        [Header("Move Settings")]
        public float moveSpeed = 3f;

        [HideInInspector]
        public bool isCleared = false;

        public override void Interaction()
        {
            // ⭐ V키 상호작용 시작
            MoveToTarget();
        }

        public void MoveToTarget()
        {
            if (isCleared) return;

            if (targetPoint == null)
            {
                Debug.LogError($"{name} : TargetPoint가 지정되지 않았습니다.");
                return;
            }

            isCleared = true;
            Debug.Log($"[PushItem] {name} moving to target");
            StartCoroutine(MoveRoutine());
        }

        private IEnumerator MoveRoutine()
        {
            while (Vector3.Distance(transform.position, targetPoint.position) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPoint.position,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = targetPoint.position;
            Debug.Log($"[PushItem] {name} reached target");
        }
    }
}