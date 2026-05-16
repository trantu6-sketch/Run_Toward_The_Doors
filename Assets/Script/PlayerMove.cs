using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public float groundCheckDistance = 1.1f; // Độ dài tia check đất (từ tâm nhân vật xuống)
    public LayerMask groundMask; // Layer của mặt đất

    [Header("Mobile UI Controls")]
    public MobileJoystick joystick;
    public MobileButton jumpButton;

    private Rigidbody rb;
    private Animator anim;
    private bool isGrounded;
    private bool isDead = false;

    public void SetupAnimator(Animator newAnim)
    {
        anim = newAnim;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Nếu PlayerSkinController chưa gán Animator cho chúng ta
        if (anim == null)
        {
            Animator[] allAnims = GetComponentsInChildren<Animator>();
            foreach (var a in allAnims)
            {
                // Bỏ qua Animator nằm ở gốc (Player) và ưu tiên Animator của Skin đang được bật
                if (a.gameObject != this.gameObject && a.gameObject.activeInHierarchy)
                {
                    anim = a;
                    break;
                }
            }
            // Nếu vẫn không có, lấy Animator ở gốc làm dự phòng
            if (anim == null) anim = GetComponent<Animator>();
        }

        // Khóa xoay của Rigidbody để nhân vật không bị ngã lăn ra
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        if (isDead) return; // Nếu đã chết thì không cho thao tác nữa

        CheckGrounded();
        Jump();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        Move();
    }

    void CheckGrounded()
    {
        // Bắn 1 tia từ tâm nhân vật hướng xuống dưới để xem có chạm layer Đất không
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }

    void Move()
    {
        float moveX = 0f;
        float moveZ = 0f;

        // Lấy input từ Mobile Joystick (nếu có)
        if (joystick != null && joystick.InputDirection.magnitude > 0.05f)
        {
            // Trục X của Joystick (kéo trái/phải) điều khiển trục Z (Tiến/Lùi) -> Giống phím A/D
            moveZ = joystick.InputDirection.x;
            // Trục Y của Joystick (kéo lên/xuống) điều khiển trục X (Sang trái/phải) -> Giống phím W/S
            moveX = -joystick.InputDirection.y;
        }
        else if (Keyboard.current != null)
        {
            // D là tiến lên (Z+)
            if (Keyboard.current.dKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ += 1f;
            // A là đi ngược lại (Z-)
            if (Keyboard.current.aKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ -= 1f;
            // W là sang bên trái (X-)
            if (Keyboard.current.wKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
            // S là sang bên phải (X+)
            if (Keyboard.current.sKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
        }

        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Xoay mặt nhân vật theo hướng di chuyển
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }

        // Di chuyển bằng Rigidbody (Sử dụng linearVelocity cho Unity 6)
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        // Cập nhật Animation chạy/đứng
        anim.SetFloat("Speed", moveDirection.magnitude);
    }

    void Jump()
    {
        bool jumpInput = false;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpInput = true;
        }

        if (jumpButton != null && jumpButton.WasPressedThisFrame)
        {
            jumpInput = true;
        }

        // Nếu có lệnh nhảy và đang đứng trên mặt đất
        if (jumpInput && isGrounded)
        {
            // Thêm lực nhảy thẳng đứng lên
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Kích hoạt Animation Nhảy
            anim.SetTrigger("Jump");
        }
    }

    // Bạn có thể gọi hàm này từ Script khác (VD: Enemy tấn công trúng)
    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            anim.SetTrigger("Die");
            // Dừng hẳn vận tốc khi chết
            rb.linearVelocity = Vector3.zero;
            
            // Bắt đầu đếm ngược thời gian chờ animation Die chạy xong rồi gọi UI
            StartCoroutine(WaitAndShowDeathUI());
        }
    }

    private System.Collections.IEnumerator WaitAndShowDeathUI()
    {
        // Đợi 2 giây cho animation chết diễn ra (bạn có thể thay đổi số 2f theo ý muốn)
        yield return new WaitForSeconds(2f);

        // Gọi GameManager hiện màn hình YOU DIE
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowDeathUI();
        }
    }

    public void Respawn()
    {
        // Đặt lại vị trí về 0,0,0
        transform.position = Vector3.zero;
        
        // Reset trạng thái sống
        isDead = false;
        
        // Kích hoạt lại animation đứng yên / chạy bình thường
        anim.Play("Idle"); // Bạn có thể cần đổi tên "Idle" thành tên animation đứng yên thật của bạn
        
        // Xóa sạch trigger cũ để không bị vướng
        anim.ResetTrigger("Die");
    }
}
