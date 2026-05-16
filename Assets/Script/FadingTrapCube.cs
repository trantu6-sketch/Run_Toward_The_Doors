using System.Collections;
using UnityEngine;

public class FadingTrapCube : MonoBehaviour
{
    [Header("Trap Settings")]
    [Tooltip("Độ mạnh của hiệu ứng rung lắc")]
    public float shakeIntensity = 0.05f;
    [Tooltip("Thời gian rung lắc và mờ dần trước khi biến mất (giây)")]
    public float duration = 1f;
    [Tooltip("Tag của đối tượng có thể kích hoạt bẫy")]
    public string playerTag = "Player";

    private bool isTriggered = false;
    private Vector3 originalPosition;
    private Renderer meshRenderer;
    private Color originalColor;

    void Start()
    {
        originalPosition = transform.position;
        meshRenderer = GetComponent<Renderer>();
        
        if (meshRenderer != null)
        {
            // Lưu lại màu gốc của vật thể
            originalColor = meshRenderer.material.color;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra xem đối tượng chạm vào có phải là Player không
        if (!isTriggered && collision.gameObject.CompareTag(playerTag))
        {
            isTriggered = true;
            StartCoroutine(TrapSequence());
        }
    }

    private IEnumerator TrapSequence()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 1. Hiệu ứng rung lắc (Shake)
            // Tạo một vị trí ngẫu nhiên xung quanh vị trí gốc
            transform.position = originalPosition + Random.insideUnitSphere * shakeIntensity;

            // 2. Hiệu ứng mờ dần (Fade)
            if (meshRenderer != null && meshRenderer.material.HasProperty("_Color"))
            {
                // Tính toán độ mờ (alpha) giảm dần từ gốc xuống 0
                float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / duration);
                Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                meshRenderer.material.color = newColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Đợi đến frame tiếp theo
        }

        // Đảm bảo đưa vị trí về ban đầu (tốt cho việc nếu bạn muốn tái sử dụng cube sau này)
        transform.position = originalPosition;
        
        // Đặt alpha về 0 hoàn toàn ở frame cuối cùng
        if (meshRenderer != null && meshRenderer.material.HasProperty("_Color"))
        {
            meshRenderer.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }

        // 3. Biến mất (Vô hiệu hóa GameObject)
        gameObject.SetActive(false);
    }
}
