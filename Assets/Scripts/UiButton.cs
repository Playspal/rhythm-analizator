using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiButton : MonoBehaviour, IPointerDownHandler
{
    public event Action OnPress;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPress?.Invoke();
    }
}
