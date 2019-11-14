using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkipScrollHandler : MonoBehaviour, IBeginDragHandler,  IDragHandler, IEndDragHandler, IScrollHandler
{
    private ScrollRect mainScroll;

    private void Start ()
    {
        mainScroll = GetComponentInParent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (mainScroll == null)
            return;

        mainScroll.OnBeginDrag(eventData);
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (mainScroll == null)
            return;

        mainScroll.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (mainScroll == null)
            return;

        mainScroll.OnEndDrag(eventData);
    }


    public void OnScroll(PointerEventData data)
    {
        if (mainScroll == null)
            return;

        mainScroll.OnScroll(data);
    }
}