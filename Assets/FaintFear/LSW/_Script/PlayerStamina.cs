using FaintFear;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina = 0f;
    [SerializeField] private float minStaminaToStart = 30.0f;

    [Header("Rate Settings")]
    [SerializeField] private float consumeRate = 20f;
    [SerializeField] private float recoverRate = 15f;

    private bool onShift = false; // 사용자가 쉬프트를 누르고 있는가
    private PlayerMove playerMove;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        currentStamina = maxStamina;
    }

    private void OnEnable() { playerMove.OnSprintEvent += ToggleShift; }
    private void OnDisable() { playerMove.OnSprintEvent -= ToggleShift; }

    private void Update()
    {
        HandleStaminaLogic();
    }

    private void HandleStaminaLogic()
    {
        // 1. 달리기 상태 판정 로직
        if (onShift)
        {
            // 걷는 중인데 스태미나가 30 이상이면 달리기 시작
            if (playerMove.currentState == PlayerState.Walk && currentStamina >= minStaminaToStart)
            {
                playerMove.SetState(PlayerState.Run);
            }

            // 달리는 중인데 스태미나가 0 이하가 되면 강제 멈춤
            if (playerMove.currentState == PlayerState.Run && currentStamina <= 0f)
            {
                playerMove.SetState(PlayerState.Walk);
                ToggleShift();
            }
        }
        else
        {
            // 쉬프트를 떼면 즉시 걷기
            playerMove.SetState(PlayerState.Walk);
        }

        // 2. 실제 수치 변화 적용
        if (playerMove.currentState == PlayerState.Run)
        {
            ConsumeStamina();
        }
        else
        {
            RecoverStamina();
        }
    }

    private void ConsumeStamina()
    {
        currentStamina -= consumeRate * Time.deltaTime;
        currentStamina = Mathf.Max(currentStamina, 0f);
    }

    private void RecoverStamina()
    {
        currentStamina += recoverRate * Time.deltaTime;
        currentStamina = Mathf.Min(currentStamina, maxStamina);
    }

    private void ToggleShift()
    {
        onShift = !onShift;
    }
}