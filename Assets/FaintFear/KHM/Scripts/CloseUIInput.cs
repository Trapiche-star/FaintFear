using UnityEngine;
namespace FaintFear
{
    public static class UIState
    {
        public static bool IsUIOpen = false;
    }

    /// <summary>
    /// E키를 눌렀을 때 UI를 끄는 클래스 
    /// </summary>
    public class CloseUIInput : MonoBehaviour
    {
        private bool useInteractKey = true; // E
        private PlayerMove playerMove;

        private PlayerInputAction input;

        #region Unity Event Method
        private void Awake()
        {
            playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();

            input = new PlayerInputAction();

            if (useInteractKey)
                input.Player.Interaction.performed += _ => Close();
        }

        private void OnEnable()
        {
            input.Enable();
            UIState.IsUIOpen = true;
        }

        private void OnDisable()
        {
            input.Disable();
            UIState.IsUIOpen = false;
        }
        #endregion

        //UI 끄기
        private void Close()
        {
            //UI 끄기
            gameObject.SetActive(false);

            //플레이어 움직임 풀기
            playerMove.enabled = true;

            //정신력 시스템 on
            PlayerStatus.Instance.isMentalSystemActive = true;
            //배터리 시스템 on
            PlayerStatus.Instance.isBatteryActive = true;
        }
    }
}