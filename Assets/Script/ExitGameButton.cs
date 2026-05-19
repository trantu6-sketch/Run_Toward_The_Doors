using UnityEngine;

public class ExitGameButton : MonoBehaviour
{
    /// <summary>
    /// Hàm này được gọi khi bấm vào nút Thoát.
    /// Nó sẽ thoát game khi đã build, và dừng Play mode nếu đang chạy trong Unity Editor.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Đang thoát game...");

#if UNITY_EDITOR
        // Dừng Play mode khi đang chạy trong Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Thoát game khi đã build ra file chạy (exe, apk, v.v.)
        Application.Quit();
#endif
    }
}
