using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiLine : MonoBehaviour
{
    [SerializeField] private UiLineItem[] _items;

    public void SetItemFull(int index, bool isFull)
    {
        _items[index].SetFull(isFull);
    }
}
