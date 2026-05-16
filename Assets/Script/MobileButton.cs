using UnityEngine;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isPressed = false;
    private bool wasPressedThisFrame = false;

    public bool IsPressed => isPressed;

    // Giả lập InputSystem's wasPressedThisFrame
    public bool WasPressedThisFrame
    {
        get
        {
            if (wasPressedThisFrame)
            {
                wasPressedThisFrame = false; // Chỉ trả về true 1 lần (1 frame)
                return true;
            }
            return false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        wasPressedThisFrame = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}
