using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform levelPanel;        // Kéo Panel chứa các nút chọn Level vào đây
    public Button closeButton;              // Nút tắt Bảng chọn Level (nếu có)
    
    [Header("Level Buttons Setup")]
    [Tooltip("Kéo thả tất cả các nút bấm level theo thứ tự từ 1 đến N vào đây")]
    public Button[] levelButtons;           

    [Header("Visual Settings")]
    public Color lockedColor = Color.gray;  // Màu của level chưa mở khóa
    public Color unlockedColor = Color.white; // Màu của level đã mở khóa
    
    [Header("Scene Settings")]
    [Tooltip("Tiền tố của tên Scene, ví dụ 'Level' thì game sẽ tự load 'Level1', 'Level2'...")]
    public string scenePrefix = "Level";    

    private int unlockedLevel;

    private void Start()
    {
        // Ẩn Panel lúc đầu
        if (levelPanel != null)
        {
            levelPanel.localScale = Vector3.zero;
            levelPanel.gameObject.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseLevelPanel);
        }

        // Khởi tạo các sự kiện cho nút level
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; // Level bắt đầu từ 1
            Button btn = levelButtons[i];

            if (btn != null)
            {
                // Xóa các sự kiện cũ nếu có để tránh lỗi
                btn.onClick.RemoveAllListeners();
                
                // Gắn sự kiện khi nhấn nút
                btn.onClick.AddListener(() => LoadLevel(levelIndex));
            }
        }
    }

    public void OpenLevelPanel()
    {
        // Lấy thông tin màn chơi cao nhất đã mở khóa
        // Mặc định là 1 nếu chưa từng chơi
        unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Cập nhật trạng thái của các nút
        UpdateLevelButtonsState();

        // Bật panel và chạy hiệu ứng DOTween
        if (levelPanel != null)
        {
            levelPanel.gameObject.SetActive(true);
            levelPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }

    public void CloseLevelPanel()
    {
        // Thu nhỏ panel và tắt đi
        if (levelPanel != null)
        {
            levelPanel.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                levelPanel.gameObject.SetActive(false);
            });
        }
    }

    private void UpdateLevelButtonsState()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            Button btn = levelButtons[i];

            if (btn != null)
            {
                Image btnImage = btn.GetComponent<Image>();

                if (levelIndex <= unlockedLevel)
                {
                    // Level đã mở khóa
                    btn.interactable = true;
                    if (btnImage != null) btnImage.color = unlockedColor;
                }
                else
                {
                    // Level chưa mở khóa
                    btn.interactable = false;
                    if (btnImage != null) btnImage.color = lockedColor;
                }
            }
        }
    }

    private void LoadLevel(int levelIndex)
    {
        // Tạo tên Scene, ví dụ: Level1, Level2
        string sceneName = scenePrefix + levelIndex;
        
        // Bạn có thể tắt nhạc ở Home Scene (nếu có) trước khi load
        // Và tắt Time.timeScale nếu đang bị dừng
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }
}
