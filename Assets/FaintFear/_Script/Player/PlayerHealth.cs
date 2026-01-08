using System;
using UnityEngine;
using UnityEngine.Events;

namespace FaintFear
{
    //정신력 상태 
    public enum MentalState
    {
        Stable,     // 안정
        Uneasy,     // 불안
        Tension,    // 긴장
        Fear,       // 공포
        Panic       // 패닉
    }

    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        #region Variables
        //참조
        private Flashlight flashlight;

        private float mental;
        private bool isDeath = false;

        [Header("Mental Drain Factors")]
        [SerializeField] private float enemyLookDrain = 1.4f;        //적을 응시했을 때 
        [SerializeField] private float enemyChaseDrain = 1.6f;       //적이 추적해올 때
        [SerializeField] private float corpseRoomDrain = 0.4f;       //시체와 같은 방에 있었을 때 
        [SerializeField] private float flashlightDamage = 0.6f;      //손전등을 껐을 때

        [Header("Mental Heal Factors")]
        [SerializeField] private float safeZoneHeal = 3.0f;          // 안전구역 정신력 회복량
        [SerializeField] private float flashlightheal = 1.6f;        //손전등 정신력 회복

        //정신력 이벤트
        private bool isInSafeZone = false;
        private bool isInCorpseRoom = false;
        private bool isEnemyLooking = false;
        private bool isBeingChased = false;

        public UnityAction onDie;
        public event Action<MentalState> OnMentalStateChanged;

        #endregion

        #region Property
        //정신력 상태
        public MentalState CurrentMentalState { get; private set; }

        //+ MentalEffectController 호환용
        public float CurrentSanity => mental; //+

        //세이프 존
        public bool IsInSafeZone
        {
            get { return isInSafeZone; }
            set
            {
                isInSafeZone = value;
            }
        }
        public bool IsBeingChased
        {
            get { return isBeingChased; }
            set
            {
                isBeingChased = value;
            }
        }

        public bool IsEnemyLooking
        {
            get { return isEnemyLooking; }
            set
            {
                isEnemyLooking = value;
            }
        }

        //시체와 같은 방에 있을 때
        public bool IsInCorpseRoom
        {
            get { return isInCorpseRoom; }
            set
            {
                isInCorpseRoom = value;
            }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            flashlight = GetComponentInChildren<Flashlight>();
        }

        private void Start()
        {
            //초기화
            mental = PlayerStatus.Instance.currentMentalPower;
            CurrentMentalState = GetMentalState(mental);
            OnMentalStateChanged?.Invoke(CurrentMentalState);
        }

        private void Update()
        {
            //손전등 사용 가능 후부터 정신력 깎이게
            if (isDeath || !PlayerStatus.Instance.isMentalSystemActive) return;
            float mentalDelta = 0f;

            if (isInSafeZone)   //세이프 존 - 무조건 회복
            {
                mentalDelta += safeZoneHeal;
            }
            else
            {
                //손전등 on/off
                if (flashlight != null && flashlight.IsOn)
                    mentalDelta += flashlightheal;
                else
                    mentalDelta -= flashlightDamage;

                //적을 응시했을 때 
                if (isEnemyLooking)
                    mentalDelta -= enemyLookDrain;

                //적이 추적해올 때
                if (isBeingChased)
                    mentalDelta -= enemyChaseDrain;

                //시체와 같은 방에 있었을 때 
                if (isInCorpseRoom)
                    mentalDelta -= corpseRoomDrain;
            }

            ApplyMentalChange(mentalDelta);
        }
        #endregion

        #region Custom Method
        //정신력 공통 처리
        void ApplyMentalChange(float deltaPerSecond)
        {
            mental += deltaPerSecond * Time.deltaTime;
            mental = Mathf.Clamp(mental, 0f, PlayerStatus.Instance.maxMentalPower);

            PlayerStatus.Instance.SetHealth(mental);
            UpdateMentalState();

            if (mental <= 0f && !isDeath)
            {
                Die();
            }
        }

        public void TakeDamage(float damage)
        {
            mental -= damage;
            mental = Mathf.Clamp(mental, 0f, PlayerStatus.Instance.maxMentalPower);
            //+ 피격음 재생
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX("SFX_PlayerHit"); //+
            Debug.Log("데미지입음" + mental);
            PlayerStatus.Instance.SetHealth(mental);
            UpdateMentalState();

            if (mental <= 0f && isDeath == false)
            {
                Die();
            }
        }
        public void HealInstant(float amount)
        {
            if (isDeath) return;

            mental += amount;
            mental = Mathf.Clamp(mental, 0f, PlayerStatus.Instance.maxMentalPower);

            PlayerStatus.Instance.SetHealth(mental);
            UpdateMentalState();
        }

        //정신력 단계 범위 설정
        private MentalState GetMentalState(float mental)
        {
            if (mental > 80f) return MentalState.Stable;
            if (mental > 60f) return MentalState.Uneasy;
            if (mental > 30f) return MentalState.Tension;
            if (mental > 10f) return MentalState.Fear;
            return MentalState.Panic;
        }
        //정신력 단계 변경
        private void UpdateMentalState()
        {
            MentalState newState = GetMentalState(mental);

            if (newState != CurrentMentalState)
            {
                CurrentMentalState = newState;
                OnMentalStateChanged?.Invoke(newState);
            }
        }

        //죽음 처리
        private void Die()
        {
            if (isDeath) return;
            isDeath = true;

            if (SoundManager.Instance != null) //+ 게임 오버 SFX 재생
                SoundManager.Instance.PlaySFX("SFX_GameOver");

            onDie?.Invoke(); // 연출용 (UI, 사운드 등)

            // ⭐ 체크포인트에서 재시작 요청
            GameManager.Instance.RestartFromCheckpoint();
        }
        #endregion
    }
}