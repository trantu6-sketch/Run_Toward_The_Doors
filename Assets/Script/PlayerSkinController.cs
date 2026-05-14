using UnityEngine;

public class PlayerSkinController : MonoBehaviour
{
    [Header("Skin Models on Player")]
    [Tooltip("Kéo thả các object model con của Player vào đây (phải theo đúng thứ tự như trong Home Scene)")]
    public GameObject[] skinModels;

    private void Start()
    {
        // Chạy trong Start để có thể tương tác với PlayerMove an toàn
        ApplySelectedSkin();
    }

    private void ApplySelectedSkin()
    {
        // Đọc skin index đã lưu từ PlayerPrefs (mặc định là 0 nếu chưa chọn)
        int selectedIndex = PlayerPrefs.GetInt("SelectedSkin", 0);

        // Bảo vệ lỗi nếu index vượt quá số lượng skin hiện có trên Player
        if (selectedIndex >= skinModels.Length || selectedIndex < 0)
        {
            selectedIndex = 0;
        }

        Animator selectedAnim = null;

        // Tắt tất cả skin, chỉ bật skin được chọn
        for (int i = 0; i < skinModels.Length; i++)
        {
            if (skinModels[i] != null)
            {
                bool isSelected = (i == selectedIndex);
                skinModels[i].SetActive(isSelected);
                
                // Nếu đây là skin được chọn, lấy Animator của nó
                if (isSelected)
                {
                    selectedAnim = skinModels[i].GetComponent<Animator>();
                    if (selectedAnim == null)
                    {
                        selectedAnim = skinModels[i].GetComponentInChildren<Animator>();
                    }
                }
            }
        }

        // Ép PlayerMove dùng Animator của Skin vừa bật (thay vì Animator của Root)
        PlayerMove playerMove = GetComponent<PlayerMove>();
        if (playerMove != null && selectedAnim != null)
        {
            playerMove.SetupAnimator(selectedAnim);
        }
    }
}
