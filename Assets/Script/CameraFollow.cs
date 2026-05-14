using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Mục tiêu")]
    [Tooltip("Kéo thả nhân vật Player của bạn vào đây")]
    public Transform target;

    [Header("Cài đặt vị trí Camera")]
    [Tooltip("Khoảng cách lệch từ Camera đến nhân vật")]
    public Vector3 offset = new Vector3(-8f, 3f, 0f); // Mình đặt sẵn nằm bên hông luôn
    public float smoothSpeed = 5f;

    [Header("Cài đặt xoay (Lấy Player làm trung tâm)")]
    [Tooltip("Bật cái này lên thì Camera sẽ luôn luôn chĩa thẳng ống kính vào Player")]
    public bool lookAtPlayer = true;

    [Tooltip("Điểm trọng tâm trên người Player (Tăng Y để nhìn vào ngực/đầu, nếu để Y=0 sẽ nhìn xuống đất)")]
    public Vector3 centerOffset = new Vector3(0f, 1.5f, 0f);

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Di chuyển Camera mượt mà
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 2. Khóa mục tiêu, luôn lấy Player làm trung tâm khung hình
        if (lookAtPlayer)
        {
            // Cộng thêm centerOffset để Camera hướng vào thân trên của nhân vật cho đẹp
            Vector3 focusPoint = target.position + centerOffset;
            transform.LookAt(focusPoint);
        }
    }
}

