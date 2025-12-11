using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Light spotLight;              // 손전등
    private bool isOn = false;            // 현재 상태

    [SerializeField]
    private float maxBattery = 100f;       // 최대 배터리  
    [SerializeField]
    private float currentBattery;          // 현재 배터리  
    [SerializeField]
    private float batteryDrainRate = 10f; // 1초에 감소할 배터리량

    private PlayerInputAction inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputAction();

        // Flashlight Action이 눌렸을 때
        inputActions.Player.Flashlight.performed += ctx => ToggleLight();
    }

    private void Start()
    {
        //초기화
        currentBattery = 0f;
        isOn = false;
        spotLight.enabled = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (isOn)
        {
            DrainBattery();
        }
    }

    void ToggleLight()
    {
        // 배터리가 없으면 못 켠다
        if (currentBattery <= 0f)
        {
            spotLight.enabled = false;
            isOn = false;
            return;
        }

        //손전등 토글
        isOn = !isOn;
        spotLight.enabled = isOn;
    }

    //손전등 배터리 소모
    void DrainBattery()
    {
        currentBattery -= batteryDrainRate * Time.deltaTime;

        if (currentBattery <= 0f)
        {
            currentBattery = 0f;
            spotLight.enabled = false;
            isOn = false;
        }
    }
    //배터리 충전
    public void AddBattery(float amount)
    {
        currentBattery += amount;

        //최대값 넘지 않게
        if (currentBattery > maxBattery)
            currentBattery = maxBattery;
    }
}