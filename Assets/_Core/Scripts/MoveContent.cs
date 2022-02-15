using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveContent : MonoBehaviour,IUpdateSelectedHandler
{
    public bool up;
    public RectTransform thisRt;
    private bool _isPressed;
    public float moveValue = 10f;
    public bool updateSelected = true;
 
    // Start is called before the first frame update
    public void OnUpdateSelected(BaseEventData data)
    {
        MoveContentPane();
    }

    public void MoveContentPane()
    {
        var value = up ? -moveValue : moveValue;
        thisRt.anchoredPosition = new Vector2(thisRt.anchoredPosition.x,
            thisRt.anchoredPosition.y + value);
    }
}
