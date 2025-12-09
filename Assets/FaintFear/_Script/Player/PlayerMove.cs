using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 3f;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float lookSensitivity = 15f;
    [SerializeField] private float minXRotation = -85f;
    [SerializeField] private float maxXRotation = 85f;

    private PlayerInputAction inputActions;

    // 입력을 저장해둘 변수들
    private Vector2 currentMoveInput;
    private Vector2 currentLookDelta;
    private float currentXRotation = 0f;

    public Action OnInteractEvent;

    private void Awake()
    {
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
    }

    private void OnDisable()
    {
        var playerMap = inputActions.Player;

        playerMap.Move.performed -= OnMove;
        playerMap.Move.canceled -= OnMove;

        playerMap.Look.performed -= OnLook;
        playerMap.Look.canceled -= OnLook;

        playerMap.Interaction.performed -= OnInteraction;

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
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        // 저장된 입력값으로 이동 처리
        if (currentMoveInput != Vector2.zero)
        {
            Vector3 movement = (transform.right * currentMoveInput.x + transform.forward * currentMoveInput.y) * speed * Time.fixedDeltaTime;
            transform.Translate(movement, Space.World);
        }
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
        // Vector2 값을 읽어서 변수에 저장
        currentMoveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // Vector2 Delta 값을 읽어서 변수에 저장
        currentLookDelta = context.ReadValue<Vector2>();
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        // 버튼이 확실히 눌린 상태(performed)인지 확인
        if (context.performed)
        {
            OnInteractEvent?.Invoke();
        }
    }
}