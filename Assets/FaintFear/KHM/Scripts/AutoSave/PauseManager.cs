using UnityEngine;

namespace FaintFear
{
    /// <summary>
    /// 일시정지 UI를 관리하는 클래스
    /// </summary>
    public class PauseManager : MonoBehaviour
    {
        #region Variables
        private PlayerInputAction inputActions;
        public static PauseManager Instance;
        public GameObject pauseUI;
        bool isPaused;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            inputActions = new PlayerInputAction();
        }
        private void OnEnable()
        {
            inputActions.UI.Enable();
            inputActions.UI.Pause.performed += OnPause;
        }

        private void OnDisable()
        {
            inputActions.UI.Pause.performed -= OnPause;
            inputActions.UI.Disable();
        }
        #endregion

        #region Custom Method
        private void OnPause(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            //esc를 눌렀을 때 
            TogglePause();
        }

        //일시정지 토글
        public void TogglePause()
        {
            isPaused = !isPaused;

            Time.timeScale = isPaused ? 0f : 1f;
            pauseUI.SetActive(isPaused);
        }

        //이어하기
        public void Resume()
        {
            if (!isPaused) return;
            TogglePause();
        }

        //메인메뉴
        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            GameManager.Instance.GoToMainMenu();
        }
        #endregion
    }
}
