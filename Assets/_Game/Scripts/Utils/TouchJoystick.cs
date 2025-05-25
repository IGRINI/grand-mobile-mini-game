using UnityEngine;
using UnityEngine.EventSystems;

public class TouchJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    private Vector2 inputVector = Vector2.zero;

    public Vector2 Direction => inputVector;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / background.sizeDelta.x) * 2f;
            pos.y = (pos.y / background.sizeDelta.y) * 2f;

            inputVector = pos.magnitude > 1f ? pos.normalized : pos;

            handle.anchoredPosition = new Vector2(
                inputVector.x * background.sizeDelta.x / 2f,
                inputVector.y * background.sizeDelta.y / 2f
            );
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
} 