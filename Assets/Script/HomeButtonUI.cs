using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class HomeButtonUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Panel chứa bảng thông báo (chứa dòng chữ, nút Yes, No)")]
    [SerializeField] private GameObject confirmPanel;
    
    [Tooltip("RectTransform của panel để làm hiệu ứng DOTween")]
    [SerializeField] private RectTransform panelRect;
    
    [Tooltip("Nút Home (Prefab ButtonHome)")]
    [SerializeField] private Button buttonHome;
    
    [Tooltip("Nút Yes trên bảng thông báo")]
    [SerializeField] private Button buttonYes;
    
    [Tooltip("Nút No trên bảng thông báo")]
    [SerializeField] private Button buttonNo;

    [Header("Player Reference")]
    [Tooltip("Kéo script PlayerMove (từ nhân vật) vào đây để khóa di chuyển")]
    [SerializeField] private PlayerMove playerMove;

    [Header("Settings")]
    [Tooltip("Tên của Scene Home bạn muốn chuyển tới")]
    [SerializeField] private string homeSceneName = "Home"; 
    
    [Tooltip("Thời gian chạy hiệu ứng bật/tắt (giây)")]
    [SerializeField] private float tweenDuration = 0.3f;

    private void Start()
    {
        // 1. Đảm bảo bảng thông báo bị ẩn lúc bắt đầu
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(false);
            panelRect.localScale = Vector3.zero;
        }

        // 2. Gán sự kiện (Event) cho các nút khi nhấn vào
        if (buttonHome != null) buttonHome.onClick.AddListener(ShowConfirmPanel);
        if (buttonYes != null) buttonYes.onClick.AddListener(OnYesClicked);
        if (buttonNo != null) buttonNo.onClick.AddListener(OnNoClicked);
    }

    private void ShowConfirmPanel()
    {
        // Bật GameObject panel lên
        confirmPanel.SetActive(true);
        
        // Đặt kích thước bắt đầu từ 0
        panelRect.localScale = Vector3.zero;
        
        // Dùng DOTween phóng to panel lên kích thước thật (1, 1, 1) với hiệu ứng OutBack (hơi nảy ra ngoài)
        // SetUpdate(true) giúp DOTween chạy được ngay cả khi game bị pause (Time.timeScale = 0)
        panelRect.DOScale(Vector3.one, tweenDuration).SetEase(Ease.OutBack).SetUpdate(true);
        
        // Dừng thời gian trong game
        Time.timeScale = 0f;

        // Khóa di chuyển của người chơi (nếu có gán PlayerMove)
        if (playerMove != null)
        {
            playerMove.enabled = false;
        }
    }

    private void OnYesClicked()
    {
        // Khôi phục lại thời gian game nếu có dùng Time.timeScale = 0f ở trên
        Time.timeScale = 1f;

        // Chuyển tới Scene Home
        SceneManager.LoadScene(homeSceneName);
    }

    private void OnNoClicked()
    {
        // Dùng DOTween thu nhỏ panel về 0 với hiệu ứng InBack
        panelRect.DOScale(Vector3.zero, tweenDuration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            // Khi hiệu ứng chạy xong thì tắt hẳn GameObject đi
            confirmPanel.SetActive(false);
            
            // Khôi phục lại thời gian game
            Time.timeScale = 1f; 

            // Mở khóa di chuyển của người chơi
            if (playerMove != null)
            {
                playerMove.enabled = true;
            }
        });
    }

    private void OnDestroy()
    {
        // Dọn dẹp các event khi object bị hủy để tránh lỗi memory leak
        if (buttonHome != null) buttonHome.onClick.RemoveListener(ShowConfirmPanel);
        if (buttonYes != null) buttonYes.onClick.RemoveListener(OnYesClicked);
        if (buttonNo != null) buttonNo.onClick.RemoveListener(OnNoClicked);
        
        // Dừng các hiệu ứng DOTween đang chạy dở trên panelRect (nếu có)
        if (panelRect != null) DOTween.Kill(panelRect);
    }
}
