using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveContent : MonoBehaviour,IUpdateSelectedHandler,IPointerDownHandler,IPointerUpHandler
{
    public bool up;
    public RectTransform thisRt;
    private bool isPressed;
 
    // Start is called before the first frame update
    public void OnUpdateSelected(BaseEventData data)
    {
        if (isPressed)
        {
            MoveContentPane();
        }
    }
    public void OnPointerDown(PointerEventData data)
    {
        isPressed = true;
    }
    public void OnPointerUp(PointerEventData data)
    {
        isPressed = false;
    }

    private void MoveContentPane()
    {
        var value = up ? -10f : 10f;
        thisRt.anchoredPosition = new Vector2(thisRt.anchoredPosition.x,
            thisRt.anchoredPosition.y + value);
    }
}
