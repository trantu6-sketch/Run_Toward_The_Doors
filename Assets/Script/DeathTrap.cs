using UnityEngine;

public class DeathTrap : MonoBehaviour
{
    // Hàm này tự động chạy khi có một vật thể (có Rigidbody/Collider) đi XUYÊN QUA vùng Trigger
    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem vật thể vừa chạm vào có mang script PlayerMove (tức là nhân vật) hay không
        PlayerMove player = other.GetComponent<PlayerMove>();

        if (player != null)
        {
            // Nếu đúng là Player, gọi hàm Die() đã được viết sẵn trong script PlayerMove
            // Hàm Die() này đã có sẵn tính năng khóa di chuyển và gọi Animation chết
            player.Die();
        }
    }
}
