using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FaintFear
{
    /// <summary>
    /// 플레이어 이동 및 시점 조작 처리
    /// </summary>
    
    // 이 컴포넌트가 있으면 CharacterController를 자동으로 추가함
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour
    {
        #region Variables
        [Header("Movement Settings")]
        [SerializeField] private float speed = 3f;
        [SerializeField] private float gravity = -9.81f; // 중력 가속도 추가
        [SerializeField] private float jumpHeight = 1.0f; // 필요시 점프 높이

        [Header("Look Settings")]
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private float lookSensitivity = 15f;
        [SerializeField] private float minXRotation = -85f;
        [SerializeField] private float maxXRotation = 85f;

        private CharacterController controller; // CharacterController 참조 변수
        private PlayerInputAction inputActions;

        // 입력을 저장해둘 변수들
        private Vector2 currentMoveInput;
        private Vector2 currentLookDelta;
        private float currentXRotation = 0f;

        // 중력 처리를 위한 속도 변수
        private Vector3 velocity;

        public Action OnInteractEvent;
        public Action OnFlashLightEvent;

        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // CharacterController 컴포넌트 가져오기
            controller = GetComponent<CharacterController>();

            if (cameraRoot == null) cameraRoot = transform.GetChild(0);
            inputActions = new PlayerInputAction();
        }

        private void OnEnable()
        {
            var playerMap = inputActions.Player;
            playerMap.Enable();

            playerMap.Move.performed += OnMove;
            playerMap.Move.canceled += OnMove;

            playerMap.Look.performed += OnLook;
            playerMap.Look.canceled += OnLook;

            playerMap.Interaction.performed += OnInteraction;
            playerMap.Flashlight.performed += OnFlashLightInteraction;
        }


        private void OnDisable()
        {
            var playerMap = inputActions.Player;

            playerMap.Move.performed -= OnMove;
            playerMap.Move.canceled -= OnMove;

            playerMap.Look.performed -= OnLook;
            playerMap.Look.canceled -= OnLook;

            playerMap.Interaction.performed -= OnInteraction;
            playerMap.Flashlight.performed -= OnFlashLightInteraction;
            playerMap.Disable();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            Look();
            Move();
        }

        #endregion

        #region Custom Method
        void Move()
        {
            //  바닥 체크 및 중력 초기화
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // 입력에 따른 수평 이동 처리
            if (currentMoveInput != Vector2.zero)
            {
                // 방향 계산
                Vector3 moveDir = transform.right * currentMoveInput.x + transform.forward * currentMoveInput.y;

                controller.Move(moveDir * speed * Time.deltaTime);
            }

            // 중력 적용 (수직 이동)
            velocity.y += gravity * Time.deltaTime;

            // 중력 이동 적용 (낙하)
            controller.Move(velocity * Time.deltaTime);
        }

        void Look()
        {
            // 저장된 델타값으로 회전 처리
            float yRotation = currentLookDelta.x * lookSensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up * yRotation);

            float mouseY = currentLookDelta.y * lookSensitivity * Time.deltaTime;
            currentXRotation -= mouseY;
            currentXRotation = Mathf.Clamp(currentXRotation, minXRotation, maxXRotation);

            if (cameraRoot != null)
            {
                cameraRoot.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            currentMoveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            currentLookDelta = context.ReadValue<Vector2>();
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInteractEvent?.Invoke();
            }
        }

        private void OnFlashLightInteraction(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnFlashLightEvent?.Invoke();
            }    
        }
        #endregion
    }
}
