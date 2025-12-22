using FaintFear;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 정신력에 따른 효과(비네팅,효과음 등)를 주는 클래스
/// </summary>


public class MentalEffectController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private CinemachineCamera vcam;

    private CinemachineBasicMultiChannelPerlin noise;

    //효과
    private Vignette vignette;      //비네팅
    private MotionBlur motionBlur;  //모션 블러

    [Header("Smooth")]
    [SerializeField] private float vignetteSmoothSpeed = 3f;
    [SerializeField] private float motionBlurSmoothSpeed = 3f;
    [SerializeField] private float shakeSmoothSpeed = 3f;

    private float targetVignetteIntensity;
    private Color targetVignetteColor;
    private float targetMotionBlur;
    private float targetShake;

    #region Mental State Settings
    [System.Serializable]
    public class MentalVisualSetting
    {
        public float vignetteIntensity;
        public Color vignetteColor;
        public float cameraShake;
        public float motionBlur;
    }

    [Header("Mental State Visuals")]
    [SerializeField] private MentalVisualSetting uneasy;
    [SerializeField] private MentalVisualSetting tension;
    [SerializeField] private MentalVisualSetting fear;
    [SerializeField] private MentalVisualSetting panic;
    #endregion

    #region Unity Event Method
    private void Awake()
    {
        // Noise 컴포넌트 가져오기
        noise = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            Debug.LogError("❌ CinemachineBasicMultiChannelPerlin을 찾을 수 없습니다!");
        }
        else
        {
            Debug.Log($"✅ Noise 찾음! 초기 Amplitude: {noise.AmplitudeGain}");
        }

        // Post Processing - CinemachineVolumeSettings에서 가져오기
        var volumeSettings = vcam.GetComponent<CinemachineVolumeSettings>();
        if (volumeSettings != null && volumeSettings.Profile != null)
        {
            Debug.Log("✅ CinemachineVolumeSettings 찾음!");

            if (volumeSettings.Profile.TryGet(out vignette))
            {
                vignette.intensity.overrideState = true;
                vignette.color.overrideState = true;
                vignette.active = true;
                Debug.Log($"✅ Vignette 찾음! Override: {vignette.intensity.overrideState}");
            }
            else
            {
                Debug.LogError("❌ Vignette를 찾을 수 없습니다!");
            }

            if (volumeSettings.Profile.TryGet(out motionBlur))
            {
                motionBlur.intensity.overrideState = true;
                motionBlur.active = true;
                Debug.Log($"✅ MotionBlur 찾음! Override: {motionBlur.intensity.overrideState}");
            }
            else
            {
                Debug.LogError("❌ MotionBlur를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.LogError("❌ CinemachineVolumeSettings 또는 Profile을 찾을 수 없습니다!");
        }
    }

    private void Start()
    {
        // 3초 후 자동 테스트
        Invoke("TestPanicAuto", 3f);
        Invoke("TestStableAuto", 6f);
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnMentalStateChanged += OnMentalStateChanged;
            Debug.Log("✅ PlayerHealth 이벤트 구독 완료");
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnMentalStateChanged -= OnMentalStateChanged;
        }
    }

    private void Update()
    {
        // 비네팅
        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(
                vignette.intensity.value,
                targetVignetteIntensity,
                Time.deltaTime * vignetteSmoothSpeed
            );

            vignette.color.value = Color.Lerp(
                vignette.color.value,
                targetVignetteColor,
                Time.deltaTime * vignetteSmoothSpeed
            );
        }

        // 모션 블러
        if (motionBlur != null)
        {
            motionBlur.intensity.value = Mathf.Lerp(
                motionBlur.intensity.value,
                targetMotionBlur,
                Time.deltaTime * motionBlurSmoothSpeed
            );
        }

        // 카메라 흔들림
        if (noise != null)
        {
            noise.AmplitudeGain = Mathf.Lerp(
                noise.AmplitudeGain,
                targetShake,
                Time.deltaTime * shakeSmoothSpeed
            );
        }
    }
    #endregion

    void OnMentalStateChanged(MentalState state)
    {
        Debug.Log($"🔔 Mental State 변경: {state}");

        switch (state)
        {
            //안정
            case MentalState.Stable:
                ResetEffects();
                Debug.Log("😌 Stable - 효과 리셋");
                break;

            //불안
            case MentalState.Uneasy:
                ApplySetting(uneasy);
                Debug.Log($"😰 Uneasy - Shake:{uneasy.cameraShake}, Blur:{uneasy.motionBlur}");
                break;

            //긴장
            case MentalState.Tension:
                ApplySetting(tension);
                Debug.Log($"😨 Tension - Shake:{tension.cameraShake}, Blur:{tension.motionBlur}");
                AudioManager.Instance.Play("배경음");
                break;

            //공포
            case MentalState.Fear:
                ApplySetting(fear);
                Debug.Log($"😱 Fear - Shake:{fear.cameraShake}, Blur:{fear.motionBlur}");
                AudioManager.Instance.Play("배경음");
                AudioManager.Instance.Play("심박 소리");
                break;

            //패닉
            case MentalState.Panic:
                ApplySetting(panic);
                Debug.Log($"😵 Panic - Shake:{panic.cameraShake}, Blur:{panic.motionBlur}");
                AudioManager.Instance.Play("배경음");
                AudioManager.Instance.Play("심박 소리");
                AudioManager.Instance.Play("숨소리");
                break;
        }
    }

    private void ApplySetting(MentalVisualSetting setting)
    {
        targetVignetteIntensity = setting.vignetteIntensity;
        targetVignetteColor = setting.vignetteColor;
        targetMotionBlur = setting.motionBlur;
        targetShake = setting.cameraShake;

        Debug.Log($"🎯 목표 설정 - Vignette:{targetVignetteIntensity}, Shake:{targetShake}, Blur:{targetMotionBlur}");
    }

    private void ResetEffects()
    {
        targetVignetteIntensity = 0f;
        targetVignetteColor = Color.black;
        targetMotionBlur = 0f;
        targetShake = 0f;
    }

    // 자동 테스트
    private void TestPanicAuto()
    {
        Debug.Log("🧪🧪🧪 3초 후 자동 Panic 테스트!");
        OnMentalStateChanged(MentalState.Panic);
    }

    private void TestStableAuto()
    {
        Debug.Log("🧪🧪🧪 6초 후 자동 Stable 테스트!");
        OnMentalStateChanged(MentalState.Stable);
    }

    // 수동 테스트
    [ContextMenu("Test Panic")]
    private void TestPanic()
    {
        OnMentalStateChanged(MentalState.Panic);
    }

    [ContextMenu("Test Stable")]
    private void TestStable()
    {
        OnMentalStateChanged(MentalState.Stable);
    }

    [ContextMenu("Check Values")]
    private void CheckValues()
    {
        Debug.Log("=== 현재 값 ===");
        if (vignette != null)
            Debug.Log($"Vignette: {vignette.intensity.value} (목표: {targetVignetteIntensity})");
        if (motionBlur != null)
            Debug.Log($"MotionBlur: {motionBlur.intensity.value} (목표: {targetMotionBlur})");
        if (noise != null)
            Debug.Log($"Shake: {noise.AmplitudeGain} (목표: {targetShake})");
    }
}