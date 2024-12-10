using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Karakterin hareket hızı
    public float jumpForce = 5f; // Zıplama gücü
    public float gravityMultiplier = 2f; // Yerçekimi çarpanı
    public float lookSensitivity = 100f; // Fare duyarlılığı
    public Transform cameraTransform; // Karakter kamerası
    public Transform groundCheck; // Yere temas kontrol noktası
    public float groundDistance = 0.2f; // Yere temas yarıçapı
    public LayerMask groundMask; // Yerin katmanı
    public float sprintSpeed;
    public float sprintSpeedMultiplier = 2;

    private Rigidbody rb; // Rigidbody referansı
    private Vector2 moveInput; // WASD hareket girdisi
    private Vector2 lookInput; // Fare hareket girdisi
    private bool isGrounded; // Yerde olup olmadığımızın kontrolü
    private PlayerInputs playerInput;
    private float verticalRotation;
    private float upDownRange = 80f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInputs();
    }

    void Start()
    {
        rb.freezeRotation = true; // Rigidbody'nin fiziksel dönmesini engeller
        Cursor.lockState = CursorLockMode.Locked; // Fareyi ekran ortasında kilitler
        Cursor.visible = false; // Farenin görünürlüğünü kapatır
        sprintSpeed = sprintSpeedMultiplier * speed;
    }

    private void OnEnable()
    {
        InputSubscriptions();

    }
    private void InputSubscriptions()
    {
        playerInput.Enable();

        playerInput.Player.Move.performed += OnMove;
        playerInput.Player.Move.canceled += OnMove;

        playerInput.Player.Jump.performed += OnJump;
        playerInput.Player.Jump.canceled += OnJump;

        playerInput.Player.Look.performed += OnLook;
        playerInput.Player.Look.canceled += OnLook;

        playerInput.Player.Sprint.performed += OnSprint;
        playerInput.Player.Sprint.canceled += OnSprint;
    }

    void Update()
    {
        // Yere temas kontrolü
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Kamera hareketlerini güncelle
        LookAround();
    }

    void FixedUpdate()
    {
        // Karakter hareketlerini güncelle
        Move();
    }

    void LookAround()
    {
        float mouseXRotation = lookInput.x * lookSensitivity;

        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= lookInput.y * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void Move()
    {
        // Hareket yönünü hesapla
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;      // Yatay hareket ve zıplama kuvveti
        Vector3 velocity = moveDirection * speed;
        velocity.y = rb.linearVelocity.y; // Mevcut dikey hız korunur
        rb.linearVelocity = velocity;

        // Yerçekimini manuel olarak artır
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool isSprinting = context.ReadValue<float>() > 0 ? true : false;
            speed = isSprinting ? sprintSpeed : speed;
        }
        if (context.canceled)
        {
            speed /= sprintSpeedMultiplier;
        }

    }

}
