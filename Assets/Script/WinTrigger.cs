using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class WinTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform winPanel;
    public Button retryButton;
    public Button homeButton;
    public Button nextButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip winMusic;

    [Header("Scene Transition")]
    public string nextSceneName = "Level2"; // Tên của màn chơi tiếp theo

    private bool isTriggered = false;

    private void Start()
    {
        // Ẩn Panel Win lúc bắt đầu (scale = 0)
        if (winPanel != null)
        {
            winPanel.localScale = Vector3.zero;
            winPanel.gameObject.SetActive(false);
        }

        // Đăng ký sự kiện cho các nút
        if (retryButton != null) retryButton.onClick.AddListener(OnRetryClicked);
        if (homeButton != null) homeButton.onClick.AddListener(OnHomeClicked);
        if (nextButton != null) nextButton.onClick.AddListener(OnNextClicked);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Chỉ chạy 1 lần và phải là Player va chạm
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            HandleWinCondition(other.gameObject);
        }
    }

    private void HandleWinCondition(GameObject player)
    {
        // 1. Chặn nhân vật di chuyển
        PlayerMove playerMove = player.GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.enabled = false; // Tắt script điều khiển
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero; // Triệt tiêu gia tốc
            }
        }

        // Tùy chọn: Chặn thêm animation chạy nếu cần bằng cách gọi Animator
        Animator anim = player.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
        }

        // 2. Phát âm thanh chiến thắng
        if (audioSource != null && winMusic != null)
        {
            // Nếu muốn dừng nhạc nền hiện tại, bạn có thể gọi tắt nhạc ở GameManager (nếu có)
            // GameManager.Instance.audioSource.Stop(); 
            
            audioSource.clip = winMusic;
            audioSource.Play();
        }

        // 3. Hiện giao diện Win Panel
        if (winPanel != null)
        {
            winPanel.gameObject.SetActive(true);
            // Hiệu ứng phóng to mượt mà trong 0.5s bằng DOTween
            winPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }

    private void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void OnRetryClicked()
    {
        StopAudio();
        // Load lại Scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnHomeClicked()
    {
        StopAudio();
        // Chuyển sang Scene Home
        SceneManager.LoadScene("Home");
    }

    private void OnNextClicked()
    {
        StopAudio();
        // Chuyển sang Scene tiếp theo dựa vào tên đã thiết lập
        SceneManager.LoadScene(nextSceneName);
    }
}
