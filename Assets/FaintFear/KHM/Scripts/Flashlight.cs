using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 손전등 기능을 담당하는 스크립트
    /// </summary>
    public class Flashlight : MonoBehaviour
    {
        #region Variabels
        public Light spotLight;              // 손전등
        private bool isOn = false;            // 현재 상태

        [SerializeField]
        private float batteryDrainRate = 10f; // 1초에 감소할 배터리량

        private PlayerInputAction inputActions;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            inputActions = new PlayerInputAction();

            // Flashlight Action이 눌렸을 때
            inputActions.Player.Flashlight.performed += ctx => ToggleLight();
        }

        private void Start()
        {
            //초기화
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
        #endregion

        #region Custom Method
        void ToggleLight()
        {
            // 배터리가 없으면 못 켠다
            if (PlayerStatus.Instance.currentBattery <= 0f)
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
            PlayerStatus.Instance.currentBattery -= batteryDrainRate * Time.deltaTime;

            if (PlayerStatus.Instance.currentBattery <= 0f)
            {
                PlayerStatus.Instance.currentBattery = 0f;
                spotLight.enabled = false;
                isOn = false;
            }
        }
        #endregion
    }
}


