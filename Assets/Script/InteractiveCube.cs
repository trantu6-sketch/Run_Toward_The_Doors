using UnityEngine;

public class InteractiveCube : MonoBehaviour
{
    [Header("Cài đặt trục Y")]
    [Tooltip("Khoảng cách thay đổi trục Y. Số dương để trồi lên, số âm để lún xuống.")]
    public float yOffset = -0.5f;

    [Tooltip("Tốc độ di chuyển lên/xuống")]
    public float moveSpeed = 2f;

    [Header("Tùy chọn nâng cao")]
    [Tooltip("Tích vào đây nếu bạn muốn Cube trở về vị trí cũ khi Player nhảy ra khỏi nó")]
    public bool returnWhenLeave = false;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        // Lưu lại vị trí ban đầu
        startPosition = transform.position;
        // Đặt mục tiêu ban đầu chính là vị trí hiện tại
        targetPosition = startPosition;
    }

    void Update()
    {
        // Liên tục di chuyển Cube tới vị trí mục tiêu một cách mượt mà
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
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
                // Đổi vị trí mục tiêu (thêm yOffset vào trục Y)
                targetPosition = startPosition + new Vector3(0, yOffset, 0);
            }
        }
    }

    // Khi vật thể rời đi
    void OnCollisionExit(Collision collision)
    {
        // Nếu bật tính năng trở về vị trí cũ và vật rời đi là Player
        if (returnWhenLeave && collision.gameObject.GetComponent<PlayerMove>() != null)
        {
            // Trả vị trí mục tiêu về như ban đầu
            targetPosition = startPosition;
        }
    }
}
