using FaintFear;
using Unity.Cinemachine;
using UnityEngine;
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
    private Vignette vignette;

    [Header("Smooth")]
    [SerializeField] private float vignetteSmoothSpeed = 3f;

    private float targetVignetteIntensity;
    private Color targetVignetteColor;

    #region Mental State Settings
    [System.Serializable]
    public class MentalVisualSetting
    {
        public float vignetteIntensity;
        public Color vignetteColor;
        public float cameraShake;
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
        noise = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();

        var volumeSettings = vcam.GetComponent<CinemachineVolumeSettings>();
        volumeSettings.Profile.TryGet(out vignette);
    }
    private void OnEnable()
    {
        playerHealth.OnMentalStateChanged += OnMentalStateChanged;
    }

    private void OnDisable()
    {
        playerHealth.OnMentalStateChanged -= OnMentalStateChanged;
    }

    private void Update()
    {
        // 비네팅 부드럽게 변화
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
    #endregion

    void OnMentalStateChanged(MentalState state)
    {
        switch (state)
        {
            //안정
            case MentalState.Stable:
                break;

            //불안
            case MentalState.Uneasy:
                ApplySetting(uneasy);
                break;

            //긴장
            case MentalState.Tension:
                ApplySetting(tension);
                AudioManager.Instance.Play("배경음");

                break;

            //공포
            case MentalState.Fear:
                ApplySetting(fear);
                AudioManager.Instance.Play("배경음");
                AudioManager.Instance.Play("심박 소리");
                break;

            //패닉
            case MentalState.Panic:
                ApplySetting(panic);
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

        if (noise != null)
            noise.AmplitudeGain = setting.cameraShake;
    }
}
