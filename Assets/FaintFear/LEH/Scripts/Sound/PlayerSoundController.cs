using UnityEngine;

namespace FaintFear
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerSoundController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerMove playerMove;

        [Header("Footstep")]
        [SerializeField] private AudioClip walkClip;
        [SerializeField] private AudioClip runClip;
        [SerializeField] private float stepIntervalWalk = 0.5f;
        [SerializeField] private float stepIntervalRun = 0.35f;

        private AudioSource audioSource;
        private float stepTimer;
        private bool isRunning;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (playerMove == null)
                playerMove = GetComponent<PlayerMove>();
        }

        private void OnEnable()
        {
            if (playerMove == null) return;
            playerMove.OnSprintEvent += OnSprint;
        }

        private void OnDisable()
        {
            if (playerMove == null) return;
            playerMove.OnSprintEvent -= OnSprint;
        }

        private void Update()
        {
            PlayFootstep();
        }

        private void OnSprint()
        {
            // Sprint 키 눌릴 때마다 토글된다고 가정
            isRunning = !isRunning;
        }

        private void PlayFootstep()
        {
            if (!playerMove.canMove) return;

            float moveMagnitude = playerMove.GetComponent<CharacterController>().velocity.magnitude;
            if (moveMagnitude < 0.1f) return;

            stepTimer -= Time.deltaTime;
            float interval = isRunning ? stepIntervalRun : stepIntervalWalk;

            if (stepTimer <= 0f)
            {
                audioSource.PlayOneShot(isRunning ? runClip : walkClip);
                stepTimer = interval;
            }
        }
    }
}
