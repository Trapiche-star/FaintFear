using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

namespace FaintFear
{
    /// <summary>
    /// 정신력에 따른 효과(비네팅,효과음 등)를 주는 클래스
    /// </summary>

    public class MentalEffectController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private CinemachineCamera vcam;

        [Header("Smooth")]
        [SerializeField] private float smoothSpeed = 3f;
        [SerializeField] private float wobbleSpeed = 1.5f;
        [SerializeField] private float wobbleAmount = 0.05f;

        // Cinemachine
        private CinemachineBasicMultiChannelPerlin noise;

        // Post Processing
        private Vignette vignette;
        private MotionBlur motionBlur;
        private LensDistortion lensDistortion;
        private ChromaticAberration chromaticAberration;
        private FilmGrain filmGrain;

        // Targets
        private float targetVignetteIntensity;
        private Color targetVignetteColor;

        private float targetMotionBlur;
        private float targetShake;

        private float targetLensDistortion;
        private float targetChromatic;
        private float targetGrain;

        private bool enableWobble;

        //+ 정신력 낮을 때 SFX 코루틴
        private Coroutine lowSanitySFXCoroutine; 
        //+ 정신력 낮은 BGM 상태 체크
        private bool isLowSanityBGMPlaying = false; 

        #region Mental State Settings
        [System.Serializable]
        public class MentalVisualSetting
        {
            public float vignetteIntensity;
            public Color vignetteColor;

            public float motionBlur;
            public float cameraShake;

            public float lensDistortion;
            public float chromaticAberration;
            public float filmGrain;

            public bool wobble; // 울렁거림 여부
        }

        [Header("Mental State Visuals")]
        [SerializeField] private MentalVisualSetting stable;
        [SerializeField] private MentalVisualSetting uneasy;
        [SerializeField] private MentalVisualSetting tension;
        [SerializeField] private MentalVisualSetting fear;
        [SerializeField] private MentalVisualSetting panic;
        #endregion

        private void Awake()
        {
            noise = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();

            var volumeSettings = vcam.GetComponent<CinemachineVolumeSettings>();
            if (volumeSettings != null && volumeSettings.Profile != null)
            {
                volumeSettings.Profile.TryGet(out vignette);
                volumeSettings.Profile.TryGet(out motionBlur);
                volumeSettings.Profile.TryGet(out lensDistortion);
                volumeSettings.Profile.TryGet(out chromaticAberration);
                volumeSettings.Profile.TryGet(out filmGrain);
            }
        }

        private void OnEnable()
        {
            if (playerHealth != null)
                playerHealth.OnMentalStateChanged += OnMentalStateChanged;
        }

        private void OnDisable()
        {
            if (playerHealth != null)
                playerHealth.OnMentalStateChanged -= OnMentalStateChanged;
        }

        private void Update()
        {
            float wobble = enableWobble
                ? Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount
                : 0f;

            //+ 정신력 낮을 때만 BGM과 SFX 재생
            if (playerHealth != null && playerHealth.CurrentSanity <= 40f) //+
            {
                if (SoundManager.Instance != null)
                {
                    //+ 낮은 정신력 BGM 한 번만 재생
                    if (!isLowSanityBGMPlaying)
                    {
                        SoundManager.Instance.PlayBGM("BGM_LowSanity"); //+
                        isLowSanityBGMPlaying = true; //+
                    }

                    //+ SFX 반복 재생
                    if (lowSanitySFXCoroutine == null)
                        lowSanitySFXCoroutine = StartCoroutine(PlayLowSanitySFX()); //+
                }
            }
            else
            {
                //+ 정신력 회복 시 BGM과 SFX 중지
                if (isLowSanityBGMPlaying)
                {
                    SoundManager.Instance.StopBGM(); //+
                    isLowSanityBGMPlaying = false; //+
                }

                if (lowSanitySFXCoroutine != null)
                {
                    StopCoroutine(lowSanitySFXCoroutine); //+
                    lowSanitySFXCoroutine = null; //+
                }

            }

            // Vignette
            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(
                    vignette.intensity.value,
                    targetVignetteIntensity,
                    Time.deltaTime * smoothSpeed
                );

                vignette.color.value = Color.Lerp(
                    vignette.color.value,
                    targetVignetteColor,
                    Time.deltaTime * smoothSpeed
                );
            }

            // Motion Blur
            if (motionBlur != null)
            {
                motionBlur.intensity.value = Mathf.Lerp(
                    motionBlur.intensity.value,
                    targetMotionBlur,
                    Time.deltaTime * smoothSpeed
                );
            }

            // Lens Distortion 
            if (lensDistortion != null)
            {
                lensDistortion.intensity.value = Mathf.Lerp(
                    lensDistortion.intensity.value,
                    targetLensDistortion + wobble,
                    Time.deltaTime * smoothSpeed
                );
            }

            // Chromatic Aberration
            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(
                    chromaticAberration.intensity.value,
                    targetChromatic,
                    Time.deltaTime * smoothSpeed
                );
            }

            // Film Grain
            if (filmGrain != null)
            {
                filmGrain.intensity.value = Mathf.Lerp(
                    filmGrain.intensity.value,
                    targetGrain,
                    Time.deltaTime * smoothSpeed
                );
            }

            // Camera Shake
            if (noise != null)
            {
                noise.AmplitudeGain = Mathf.Lerp(
                    noise.AmplitudeGain,
                    targetShake,
                    Time.deltaTime * smoothSpeed
                );
            }
        }

        //+ 정신력 낮을 때 반복 SFX
        private IEnumerator PlayLowSanitySFX() //+
        {
            while (true)
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySFX("SFX_Whisper"); //+
                    SoundManager.Instance.PlaySFX("SFX_Panting"); //+
                }
                yield return new WaitForSeconds(3f); //+ 3초 간격, 필요하면 조정 가능
            }
        }

        void OnMentalStateChanged(MentalState state)
        {
            switch (state)
            {
                case MentalState.Stable:
                    ApplySetting(stable);
                    break;

                case MentalState.Uneasy:
                    ApplySetting(uneasy);
                    break;

                case MentalState.Tension:
                    ApplySetting(tension);
                    //AudioManager.Instance.Play("배경음");
                    break;

                case MentalState.Fear:
                    ApplySetting(fear);
                    //AudioManager.Instance.Play("배경음");
                    //AudioManager.Instance.Play("심박 소리");
                    break;

                case MentalState.Panic:
                    ApplySetting(panic);
                    //AudioManager.Instance.Play("배경음");
                    //AudioManager.Instance.Play("심박 소리");
                    //AudioManager.Instance.Play("숨소리");
                    break;
            }
        }

        private void ApplySetting(MentalVisualSetting setting)
        {
            targetVignetteIntensity = setting.vignetteIntensity;
            targetVignetteColor = setting.vignetteColor;

            targetMotionBlur = setting.motionBlur;
            targetShake = setting.cameraShake;

            targetLensDistortion = setting.lensDistortion;
            targetChromatic = setting.chromaticAberration;
            targetGrain = setting.filmGrain;

            enableWobble = setting.wobble;
        }
    }
}