using UnityEngine;

public class SlidingTrapCubeX : MonoBehaviour
{
    [Header("Cài đặt bẫy trượt trục X")]
    [Tooltip("Khoảng cách thay đổi trục X. Số dương để di chuyển sang phải, số âm sang trái.")]
    public float xOffset = 3f;

    [Tooltip("Tốc độ di chuyển (Càng nhanh thì Player trượt càng gắt)")]
    public float moveSpeed = 5f;

    [Header("Chế độ tự động (Loop)")]
    [Tooltip("Tích vào đây để bẫy tự động chạy liên tục. Rất phù hợp làm bẫy băng chuyền trượt!")]
    public bool loop = true;
    
    [Tooltip("Thời gian chờ trước khi đảo chiều (khi dùng Loop)")]
    public float waitTime = 0.2f;

    [Header("Tùy chọn nâng cao (Khi không dùng Loop)")]
    [Tooltip("Tích vào đây nếu bạn muốn bẫy trở về vị trí cũ khi Player rơi ra khỏi nó")]
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
        // Tính toán vị trí đích đến trên trục X
        endPosition = startPosition + new Vector3(xOffset, 0, 0);
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

    // Khi có vật thể chạm vào (CỐ TÌNH KHÔNG SET PARENT ĐỂ TẠO HIỆU ỨNG TRƯỢT/BẪY)
    void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra xem vật đó có phải là Player không
        if (collision.gameObject.GetComponent<PlayerMove>() != null)
        {
            // Kiểm tra xem Player có đáp xuống từ phía trên bề mặt Cube hay không
            if (collision.transform.position.y > transform.position.y)
            {
                // Nếu không ở chế độ Loop thì thay đổi đích đến (Giật mạnh bẫy)
                if (!loop)
                {
                    // Đổi vị trí mục tiêu (thêm xOffset vào trục X)
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
            // Nếu bật tính năng trở về vị trí cũ và không dùng loop
            if (!loop && returnWhenLeave)
            {
                // Trả vị trí mục tiêu về như ban đầu
                targetPosition = startPosition;
            }
        }
    }
}
