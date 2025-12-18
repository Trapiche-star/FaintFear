using UnityEngine;

namespace FaintFear
{
    public class PushItem : MonoBehaviour
    {
        [Header("Move Target")]
        public Transform targetPoint;

        [Header("Move Settings")]
        public float moveSpeed = 3f;

        [HideInInspector]
        public bool isCleared = false;

        public void MoveToTarget()
        {
            if (isCleared) return;

            if (targetPoint == null)
            {
                Debug.LogError($"{name} : TargetPoint가 지정되지 않았습니다.");
                return;
            }

            isCleared = true;
            StartCoroutine(MoveRoutine());
        }

        System.Collections.IEnumerator MoveRoutine()
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
        }
    }
}