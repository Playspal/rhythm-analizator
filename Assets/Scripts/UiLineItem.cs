using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiLineItem : MonoBehaviour
{
    [SerializeField] private GameObject _blockEmpty;
    [SerializeField] private GameObject _blockFull;
    
    public void SetFull(bool isFull)
    {
        _blockEmpty.SetActive(!isFull);
        _blockFull.SetActive(isFull);
    }
}
