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

        // + 확대시 시점 고정용 (외부 제어)
        private bool lookLocked = false;

        public Action OnInteractEvent;
        public Action OnFlashLightEvent;
        public Action<bool> OnPushEvent;

        //움직임만 막기
        public bool canMove = true;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // CharacterController 컴포넌트 가져오기
            controller = GetComponent<CharacterController>();

            if (cameraRoot == null) cameraRoot = transform.GetChild(0);
            inputActions = new PlayerInputAction();
            canMove = true;
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

            playerMap.Push.started += OnPushStarted;
            playerMap.Push.canceled += OnPushCanceled;
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
            playerMap.Push.started -= OnPushStarted;
            playerMap.Push.canceled -= OnPushCanceled;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canMove = true;
        }

        private void Update()
        {
            // + 시점은 항상 처리
            Look();


        }

        private void FixedUpdate()
        {
            // + 이동만 선택적으로 차단
            if (!canMove)
                return;

            Move();
        }
        #endregion

        #region Custom Method
        void Move()
        {
            // 1. 바닥 체크 및 중력 초기화
            if (controller.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // 바닥에 붙어있도록 약간의 하방 힘 유지
            }

            // 2. 수평 이동 벡터 계산
            Vector3 horizontalMove = Vector3.zero;
            if (currentMoveInput != Vector2.zero)
            {
                // 방향 계산
                horizontalMove = transform.right * currentMoveInput.x + transform.forward * currentMoveInput.y;
                horizontalMove *= speed; // 속도 적용
            }

            // 3. 수직 이동(중력) 계산
            velocity.y += gravity * Time.deltaTime;

            // 4. 최종 이동 벡터 합성 (수평 + 수직)
            Vector3 finalMove = horizontalMove + Vector3.up * velocity.y;

            // 5. 실제 이동 적용
            controller.Move(finalMove * Time.deltaTime);
        }

        void Look()
        {
            // +시점 고정
            if (lookLocked) return;

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

        // + 외부(카메라 컨트롤러)에서 호출
        public void SetLookLock(bool locked)
        {
            lookLocked = locked;

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
            if (context.performed)
            {
                OnFlashLightEvent?.Invoke();
            }
        }

        private void OnPushStarted(InputAction.CallbackContext context)
        {
            OnPushEvent?.Invoke(true);
        }

        private void OnPushCanceled(InputAction.CallbackContext context)
        {
            OnPushEvent?.Invoke(false);
        }
        #endregion
    }
}