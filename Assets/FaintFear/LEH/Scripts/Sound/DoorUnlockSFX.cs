using UnityEngine;

namespace FaintFear
{
    [RequireComponent(typeof(DoorLock))]
    public class DoorUnlockSFX : MonoBehaviour
    {
        [Header("잠금 해제 후 한 번만 재생 SFX")]
        [SerializeField] private string unlockSFX = "SFX_Jumpscare01";

        [Header("Door Hinge")]
        [SerializeField] private Transform hinge;

        private bool hasPlayed = false;
        private Vector3 lastEuler;

        private void Start()
        {
            if (hinge == null)
            {
                // DoorLock에서 hinge 자동 검색
                var doorLock = GetComponent<DoorLock>();
                if (doorLock != null)
                    hinge = doorLock.transform.Find("Hinge") ?? doorLock.transform;
            }

            lastEuler = hinge != null ? hinge.localRotation.eulerAngles : Vector3.zero;
        }

        private void Update()
        {
            if (hasPlayed || hinge == null) return;

            Vector3 currentEuler = hinge.localRotation.eulerAngles;
            float delta = Vector3.Distance(currentEuler, lastEuler);

            if (delta > 0.1f) // 문이 열리기 시작하면
            {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySFX(unlockSFX);

                hasPlayed = true; // 한 번만 재생
            }

            lastEuler = currentEuler;
        }
    }
}

