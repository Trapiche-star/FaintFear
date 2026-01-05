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
        public GameObject optionsPanel;
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

            if (isPaused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        //이어하기
        public void Resume()
        {
            Debug.Log("이어하기");
            if (!isPaused) return;
            TogglePause();
        }

        //메인메뉴
        public void GoToMainMenu()
        {
            Debug.Log("메인메뉴로 이동");
            Time.timeScale = 1f;
            GameManager.Instance.GoToMainMenu();
        }

        //옵션
        public void OnOptions()
        {
            pauseUI.SetActive(false);
            optionsPanel.SetActive(true);
        }
        //옵션 창 끄기
        public void CloseOptions()
        {
            pauseUI.SetActive(true);
            optionsPanel.SetActive(false);
        }

        //게임 종료
        public void OnQuit()
        {
            Debug.Log("Quit 버튼을 눌렀습니다");
            //치팅: 저장된 데이터 리셋
            PlayerPrefs.DeleteAll();
            Application.Quit();
        }
        #endregion
    }
}
