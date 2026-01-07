using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 플레이어가 특정 영역 밖으로 나가지 못하게 제한하는 컴포넌트
    /// (플레이어는 런타임에 BindPlayer로 주입받는다)
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class TriggerRestrict : MonoBehaviour
    {
        #region Variables

        private Transform player;

        [Header("Push Back Settings")]
        [SerializeField] private float playerRadius = 0.3f;
        [SerializeField] private float pushBackOffset = 0.15f;

        [Header("UI")]
        [SerializeField] private SequenceTextManager sequenceText;

        [TextArea]
        [SerializeField]
        private string dialogueLine =
            "너무 어두워서 손전등 없이는 더 이상 나아갈 수 없다.";

        private BoxCollider boxCollider;
        private bool restrictionActive = true;
        private bool warningShown = false;

        // ⭐ 현재 Restrict가 플레이어를 밀고 있는지 상태
        public bool IsRestricting { get; private set; } = false;

        #endregion

        #region Unity

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        private void Start()
        {
            var data = SaveSystem.LoadPreview();
            bool tutorialDone = data != null && data.tutorialCompleted;

            if (tutorialDone || GameManager.TutorialCompleted)
            {
                Destroy(gameObject);
                return;
            }
        }
        private void Update()
        {
            if (GameManager.TutorialCompleted)
            {
                Destroy(gameObject);
            }
        }
        private void LateUpdate()
        {
            if (!restrictionActive) return;
            if (player == null) return;

            Bounds bounds = GetWorldBounds();
            Vector3 playerPos = player.position;

            if (!bounds.Contains(playerPos))
            {
                IsRestricting = true;
                ShowWarningOnce();

                Vector3 closestPoint = bounds.ClosestPoint(playerPos);
                Vector3 dirToCenter = (bounds.center - playerPos).normalized;

                player.position =
                    closestPoint + dirToCenter * (playerRadius + pushBackOffset);
            }
            else
            {
                IsRestricting = false;
                warningShown = false;
            }
        }

        #endregion

        #region Bind

        public void BindPlayer(GameObject playerObj)
        {
            if (playerObj == null) return;
            player = playerObj.transform;
        }

        #endregion

        #region Control

        public void SetRestriction(bool active)
        {
            restrictionActive = active;
            if (!active)
            {
                warningShown = false;
                IsRestricting = false;
            }
        }

        #endregion

        #region Helper

        private void ShowWarningOnce()
        {
            if (warningShown) return;
            if (sequenceText != null)
                sequenceText.ShowMessage(dialogueLine);
            warningShown = true;
        }

        private Bounds GetWorldBounds()
        {
            Vector3 center = transform.TransformPoint(boxCollider.center);
            Vector3 size = Vector3.Scale(boxCollider.size, transform.lossyScale);
            return new Bounds(center, size);
        }

        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            BoxCollider col = GetComponent<BoxCollider>();
            if (col == null) return;

            Gizmos.color = new Color(0f, 0.6f, 1f, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(col.center, col.size);
        }
#endif
    }
}
