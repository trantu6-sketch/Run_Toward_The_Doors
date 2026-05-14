using UnityEngine;

public class AutoDoubleDoor : MonoBehaviour
{
    [Header("Gán 2 cánh cửa vào đây")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Cài đặt thông số")]
    public float openAngle = 90f;
    public float closeAngle = 0f;
    public float smoothSpeed = 5f;

    [Header("Âm thanh cửa")]
    public AudioClip openSound;   // Kéo file âm thanh mở cửa vào đây
    public AudioClip closeSound;  // Kéo file âm thanh đóng cửa vào đây

    private bool isOpen = false;
    private AudioSource audioSource;  // Biến để gọi bộ phát âm thanh

    void Start()
    {
        // Lấy AudioSource có sẵn trên cánh cửa
        audioSource = GetComponent<AudioSource>();

        // Tắt tính năng tự động phát âm thanh khi mới bắt đầu game
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        float targetAngleL = isOpen ? -openAngle : closeAngle;
        float targetAngleR = isOpen ? openAngle : closeAngle;

        Quaternion targetRotL = Quaternion.Euler(0, targetAngleL, 0);
        Quaternion targetRotR = Quaternion.Euler(0, targetAngleR, 0);

        leftDoor.localRotation = Quaternion.Slerp(leftDoor.localRotation, targetRotL, Time.deltaTime * smoothSpeed);
        rightDoor.localRotation = Quaternion.Slerp(rightDoor.localRotation, targetRotR, Time.deltaTime * smoothSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Thêm điều kiện !isOpen để âm thanh không bị phát lặp lại nhiều lần khi nhân vật đang đứng trong vùng Trigger
        if (other.CompareTag("Player") && !isOpen)
        {
            isOpen = true;

            // Nếu có file âm thanh mở cửa thì phát nó 1 lần
            if (openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Thêm điều kiện isOpen để đảm bảo cửa đang mở thì mới phát âm thanh đóng
        if (other.CompareTag("Player") && isOpen)
        {
            isOpen = false;

            // Nếu có file âm thanh đóng cửa thì phát nó 1 lần
            if (closeSound != null)
            {
                audioSource.PlayOneShot(closeSound);
            }
        }
    }
}
