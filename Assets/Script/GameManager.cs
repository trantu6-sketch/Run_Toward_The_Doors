using UnityEngine;
using UnityEngine.UI;
using TMPro; // Thêm thư viện TextMeshPro
using DG.Tweening; // Thêm thư viện DOTween
using UnityEngine.SceneManagement; // Thêm thư viện quản lý Scene

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public RectTransform deathPanel;    // Panel YOU DIE
    public Button restartButton;        // Nút Restart

    [Header("Time Out / Lose UI")]
    public RectTransform losePanel;     // Panel YOU LOSE (Time Out)
    public Button homeButton;           // Nút Home
    public Button againButton;          // Nút Again
    public TextMeshProUGUI timerText;   // Chữ hiển thị thời gian

    [Header("Level Timer")]
    public float timeLimit = 90f;       // Thời gian tối đa của màn chơi (giây)
    private float timeRemaining;
    private bool timerIsRunning = false;
    private bool isGameOver = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip deathMusic;
    public AudioClip loseMusic;         // Nhạc khi hết giờ

    private PlayerMove playerMove;

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ẩn Panel lúc đầu (scale = 0)
        deathPanel.localScale = Vector3.zero;
        deathPanel.gameObject.SetActive(false);

        // Đăng ký sự kiện khi bấm nút Restart
        restartButton.onClick.AddListener(RestartGame);

        // Tìm Player trong Scene
        playerMove = FindFirstObjectByType<PlayerMove>();

        // Setup Time Out Panel
        if (losePanel != null)
        {
            losePanel.localScale = Vector3.zero;
            losePanel.gameObject.SetActive(false);
        }

        if (homeButton != null) homeButton.onClick.AddListener(GoHome);
        if (againButton != null) againButton.onClick.AddListener(RestartGame);

        // Khởi tạo bộ đếm thời gian
        timeRemaining = timeLimit;
        timerIsRunning = true;
        isGameOver = false;
        UpdateTimerDisplay(timeRemaining);
    }

    private void Update()
    {
        if (timerIsRunning && !isGameOver)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                UpdateTimerDisplay(timeRemaining);
                TimeOutLose();
            }
        }
    }

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timerText != null)
        {
            float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void TimeOutLose()
    {
        isGameOver = true;
        if (playerMove != null) playerMove.enabled = false; // Ngừng điều khiển Player
        
        if (losePanel != null)
        {
            losePanel.gameObject.SetActive(true);
            losePanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
        }

        if (audioSource != null && loseMusic != null)
        {
            audioSource.clip = loseMusic;
            audioSource.Play();
        }
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }

    // Hàm này sẽ được gọi từ PlayerMove khi người chơi chết
    public void ShowDeathUI()
    {
        isGameOver = true;
        timerIsRunning = false; // Dừng timer khi chết

        // Bật Panel lên
        deathPanel.gameObject.SetActive(true);

        // Hiệu ứng DOTween: Phóng to Panel từ (0,0,0) lên (1,1,1) mượt mà (OutBack)
        deathPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

        // Chơi nhạc Death
        if (audioSource != null && deathMusic != null)
        {
            audioSource.clip = deathMusic;
            audioSource.Play();
        }
    }

    public void RestartGame()
    {
        // Tắt nhạc
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // Tìm xem panel nào đang mở
        RectTransform activePanel = null;
        if (deathPanel != null && deathPanel.gameObject.activeSelf) activePanel = deathPanel;
        else if (losePanel != null && losePanel.gameObject.activeSelf) activePanel = losePanel;

        if (activePanel != null)
        {
            // Hiệu ứng DOTween: Thu nhỏ Panel về (0,0,0) (InBack)
            activePanel.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                // Tắt hẳn Panel sau khi animation thu nhỏ chạy xong
                activePanel.gameObject.SetActive(false);
                
                // Reload lại màn chơi hiện tại (Reset toàn bộ Map, Bẫy, Vị trí Player)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void GoHome()
    {
        // Tắt nhạc
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (losePanel != null && losePanel.gameObject.activeSelf)
        {
            losePanel.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                losePanel.gameObject.SetActive(false);
                Time.timeScale = 1f; // Đảm bảo thời gian chạy bình thường
                SceneManager.LoadScene("Home"); // Chuyển về màn hình Home
            });
        }
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Home");
        }
    }
}
