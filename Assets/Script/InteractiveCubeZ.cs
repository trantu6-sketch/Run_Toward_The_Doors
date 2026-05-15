using UnityEngine;

public class InteractiveCubeZ : MonoBehaviour
{
    [Header("Cài đặt trục Z")]
    [Tooltip("Khoảng cách thay đổi trục Z. Số dương để di chuyển tiến tới, số âm để lùi về.")]
    public float zOffset = 2f;

    [Tooltip("Tốc độ di chuyển")]
    public float moveSpeed = 2f;

    [Header("Chế độ tự động (Loop)")]
    [Tooltip("Tích vào đây để Cube tự động chạy đi chạy lại trên trục Z mà không cần Player")]
    public bool loop = false;
    
    [Tooltip("Thời gian chờ trước khi đảo chiều (khi dùng Loop)")]
    public float waitTime = 0.5f;

    [Header("Tùy chọn nâng cao (Khi không dùng Loop)")]
    [Tooltip("Tích vào đây nếu bạn muốn Cube trở về vị trí cũ khi Player nhảy ra khỏi nó")]
    public bool returnWhenLeave = false;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 endPosition;
    
    private bool movingToEnd = true;
    private float timer = 0f;
    private bool isWaiting = false;

    void Start()
    {
        // Lưu lại vị trí ban đầu
        startPosition = transform.position;
        // Tính toán vị trí đích đến trên trục Z
        endPosition = startPosition + new Vector3(0, 0, zOffset);
        // Đặt mục tiêu ban đầu chính là vị trí hiện tại
        targetPosition = startPosition;
    }

    void Update()
    {
        if (loop)
        {
            // Xử lý di chuyển tự động (Loop)
            HandleLoopMovement();
        }
        else
        {
            // Xử lý di chuyển theo tương tác
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void HandleLoopMovement()
    {
        if (isWaiting)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                isWaiting = false;
                timer = 0f;
                movingToEnd = !movingToEnd; // Đảo chiều di chuyển
            }
            return;
        }

        Vector3 currentTarget = movingToEnd ? endPosition : startPosition;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);

        // Nếu đã đến nơi thì chuyển sang trạng thái chờ
        if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
        {
            isWaiting = true;
        }
    }

    // Khi có vật thể chạm vào
    void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra xem vật đó có phải là Player không (thông qua Script PlayerMove)
        if (collision.gameObject.GetComponent<PlayerMove>() != null)
        {
            // Kiểm tra xem Player có đáp xuống từ phía trên bề mặt Cube hay không
            if (collision.transform.position.y > transform.position.y)
            {
                // Biến Player thành child của Cube để di chuyển theo
                collision.transform.SetParent(transform);

                // Nếu không ở chế độ Loop thì thay đổi đích đến
                if (!loop)
                {
                    // Đổi vị trí mục tiêu
                    targetPosition = endPosition;
                }
            }
        }
    }

    // Khi vật thể rời đi
    void OnCollisionExit(Collision collision)
    {
        // Kiểm tra vật rời đi là Player
        if (collision.gameObject.GetComponent<PlayerMove>() != null)
        {
            // Hủy làm child của Cube
            collision.transform.SetParent(null);

            // Nếu bật tính năng trở về vị trí cũ và không dùng loop
            if (!loop && returnWhenLeave)
            {
                // Trả vị trí mục tiêu về như ban đầu
                targetPosition = startPosition;
            }
        }
    }
}
