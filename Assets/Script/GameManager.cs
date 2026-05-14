using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Thêm thư viện DOTween
using UnityEngine.SceneManagement; // Thêm thư viện quản lý Scene

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public RectTransform deathPanel;    // Panel YOU DIE
    public Button restartButton;        // Nút Restart

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip deathMusic;

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
    }

    // Hàm này sẽ được gọi từ PlayerMove khi người chơi chết
    public void ShowDeathUI()
    {
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

        // Hiệu ứng DOTween: Thu nhỏ Panel về (0,0,0) (InBack)
        deathPanel.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            // Tắt hẳn Panel sau khi animation thu nhỏ chạy xong
            deathPanel.gameObject.SetActive(false);
            
            // Reload lại màn chơi hiện tại (Reset toàn bộ Map, Bẫy, Vị trí Player)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }
}
