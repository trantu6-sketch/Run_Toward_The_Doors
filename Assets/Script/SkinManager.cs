using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SkinManager : MonoBehaviour
{
    [Header("Skin Models in Home Scene")]
    public GameObject[] skinModels; // Kéo thả các model 3D của các skin vào đây

    [Header("UI Elements")]
    public Button nextButton;
    public Button prevButton;
    public Button saveButton;
    public Button playButton;

    [Header("Scene Settings")]
    public string gameSceneName = "SampleScene"; // Tên của Scene màn chơi chính (nhớ add vào Build Settings)

    [Header("Level Selection")]
    public LevelSelectionManager levelSelectionManager; // Tham chiếu đến bảng chọn Level

    private int currentIndex = 0;

    void Start()
    {
        // Lấy skin đã lưu (nếu chưa có thì mặc định là 0)
        currentIndex = PlayerPrefs.GetInt("SelectedSkin", 0);

        // Hiển thị skin hiện tại
        UpdateSkinDisplay();

        // Gắn sự kiện cho các nút
        if (nextButton != null) nextButton.onClick.AddListener(NextSkin);
        if (prevButton != null) prevButton.onClick.AddListener(PrevSkin);
        if (saveButton != null) saveButton.onClick.AddListener(SaveSkin);
        if (playButton != null) playButton.onClick.AddListener(PlayGame);
    }

    public void NextSkin()
    {
        currentIndex++;
        if (currentIndex >= skinModels.Length)
        {
            currentIndex = 0; // Quay lại đầu nếu vượt quá
        }
        UpdateSkinDisplay();
    }

    public void PrevSkin()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = skinModels.Length - 1; // Đi tới cuối nếu lùi quá
        }
        UpdateSkinDisplay();
    }

    private void UpdateSkinDisplay()
    {
        // Tắt hết tất cả các skin
        for (int i = 0; i < skinModels.Length; i++)
        {
            if (skinModels[i] != null)
            {
                skinModels[i].SetActive(false);
            }
        }

        // Chỉ bật skin hiện tại đang chọn
        if (skinModels.Length > 0 && skinModels[currentIndex] != null)
        {
            skinModels[currentIndex].SetActive(true);
        }
    }

    public void SaveSkin()
    {
        PlayerPrefs.SetInt("SelectedSkin", currentIndex);
        PlayerPrefs.Save();
        Debug.Log("Saved Skin Index: " + currentIndex);
    }

    public void PlayGame()
    {
        // Có thể lưu lại trước khi chơi cho chắc ăn
        SaveSkin();
        
        // Mở bảng chọn Level thay vì vào thẳng game
        if (levelSelectionManager != null)
        {
            levelSelectionManager.OpenLevelPanel();
        }
        else
        {
            Debug.LogWarning("Chưa gán LevelSelectionManager! Đang load scene mặc định.");
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
