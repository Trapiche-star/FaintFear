using FaintFear;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float minStaminaToStart = 30.0f;
    [SerializeField] private float _currentStamina = 0f;

    [Header("Rate Settings")]
    [SerializeField] private float consumeRate = 20f;
    [SerializeField] private float recoverRate = 15f;

    [Header("Exhaustion & UI Settings")]
    [SerializeField] private float regenDelay = 2.0f;
    [SerializeField] private Color normalColor = Color.white; // 평상시 색상
    [SerializeField] private Color exhaustedColor = Color.red; // 탈진시 색상

    private float currentRegenTimer = 0f;
    private bool isExhausted = false;

    private Slider staminaSlider;
    private Image sliderFillImage; // 슬라이더의 색상을 바꿀 이미지
    private bool onShift = false;
    private PlayerMove playerMove;

    public float CurrentStamina
    {
        get => _currentStamina;
        private set
        {
            _currentStamina = Mathf.Clamp(value, 0f, maxStamina);
            if (staminaSlider != null)
                staminaSlider.value = _currentStamina / maxStamina;
        }
    }

    private void Awake()
    {
        var uiObj = GameObject.Find("StaminaUI");
        if (uiObj != null)
        {
            staminaSlider = uiObj.GetComponentInChildren<Slider>();
            sliderFillImage = uiObj.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
            sliderFillImage.color = normalColor;
        }

        playerMove = GetComponent<PlayerMove>();
        CurrentStamina = maxStamina;
    }

    private void OnEnable() { playerMove.OnSprintEvent += ToggleShift; }
    private void OnDisable() { playerMove.OnSprintEvent -= ToggleShift; }

    private void Update()
    {
        HandleStaminaLogic();
        UpdateSliderColor(); // 매 프레임 색상 업데이트
    }

    private void HandleStaminaLogic()
    {
        bool isTryingToRun = onShift && playerMove.IsMovingInput && !isExhausted;

        if (isTryingToRun)
        {
            if (playerMove.currentState == PlayerState.Walk && CurrentStamina >= minStaminaToStart)
            {
                playerMove.SetState(PlayerState.Run);
            }

            if (playerMove.currentState == PlayerState.Run && CurrentStamina <= 0f)
            {
                StartExhaustion();
            }
        }
        else
        {
            if (onShift && CurrentStamina < minStaminaToStart) onShift = false;
            playerMove.SetState(PlayerState.Walk);
        }

        if (playerMove.currentState == PlayerState.Run)
        {
            ConsumeStamina();
        }
        else
        {
            if (isExhausted)
            {
                currentRegenTimer -= Time.deltaTime;
                if (currentRegenTimer <= 0) isExhausted = false;
            }
            else
            {
                RecoverStamina();
            }
        }
    }

    private void UpdateSliderColor()
    {
        if (sliderFillImage == null) return;

        if (isExhausted)
        {
            // 타이머가 진행됨에 따라 빨간색(1)에서 원래색(0)으로 변화
            float t = currentRegenTimer / regenDelay;
            sliderFillImage.color = Color.Lerp(normalColor, exhaustedColor, t);
        }
        else
        {
            // 탈진 상태가 아니면 평상시 색상 유지
            sliderFillImage.color = normalColor;
        }
    }

    private void ConsumeStamina() { CurrentStamina -= consumeRate * Time.deltaTime; }
    private void RecoverStamina() { CurrentStamina += recoverRate * Time.deltaTime; }

    private void StartExhaustion()
    {
        playerMove.SetState(PlayerState.Walk);
        onShift = false;
        isExhausted = true;
        currentRegenTimer = regenDelay;
    }

    private void ToggleShift()
    {
        // 1. 이미 Shift를 누르고 있는 상태라면, 끄는 것은 언제든 가능
        if (onShift)
        {
            onShift = false;
        }
        // 2. Shift가 꺼져 있는 상태에서 켜려고 할 때만 스태미나 체크
        else
        {
            if (CurrentStamina >= minStaminaToStart && !isExhausted)
            {
                onShift = true;
            }
            else
            {
                // 조건이 안 맞으면 onShift는 false 유지 (달리기 시작 불가)
                Debug.Log("스태미나가 부족하여 달릴 수 없습니다.");
            }
        }
    }
}