using UnityEngine;

public class MobileUIManager : MonoBehaviour
{
    [Tooltip("Tick vào nếu muốn bắt buộc bật Mobile UI để test trên PC")]
    public bool forceMobileUIForTesting = false;

    private void Start()
    {
        // Kiểm tra xem game có đang chạy trên thiết bị di động không
        bool isMobile = Application.isMobilePlatform;

        // Hoặc kiểm tra chi tiết hơn (nếu Application.isMobilePlatform trên WebGL gặp vấn đề)
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            isMobile = true;
        }

        // Bật hoặc tắt GameObject chứa UI này
        if (isMobile || forceMobileUIForTesting)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
