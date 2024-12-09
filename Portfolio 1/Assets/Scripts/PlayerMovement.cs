using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f; // Karakterin hareket hızı
    public float jumpForce = 5f; // Zıplama gücü
    public float gravityMultiplier = 2f; // Yerçekimi çarpanı

    [Header("Mouse Settings")]
    public float lookSensitivity = 100f; // Fare duyarlılığı
    public Transform cameraTransform; // Karakter kamerası

    [Header("Ground Check")]
    public Transform groundCheck; // Yere temas kontrol noktası
    public float groundDistance = 0.2f; // Yere temas yarıçapı
    public LayerMask groundMask; // Yerin katmanı

    private Rigidbody rb; // Rigidbody referansı
    private Vector2 moveInput; // WASD hareket girdisi
    private Vector2 lookInput; // Fare hareket girdisi
    private float xRotation = 0f; // Kamera dikey rotasyonu
    private bool isGrounded; // Yerde olup olmadığımızın kontrolü

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Rigidbody'nin fiziksel dönmesini engeller
        Cursor.lockState = CursorLockMode.Locked; // Fareyi ekran ortasında kilitler
        Cursor.visible = false; // Farenin görünürlüğünü kapatır
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
        lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        // Fare hareketini duyarlılık ve delta zamanı ile ölçeklendir
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        // Kameranın dikey eksendeki dönüşünü sınırla
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Kamerayı dikey olarak döndür
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Karakteri yatay olarak döndür
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        // Hareket yönünü hesapla
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Yatay hareket ve zıplama kuvveti
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
    // TODO: Look functionality will be make thru the new input system
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
