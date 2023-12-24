using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiLoopBeat : MonoBehaviour
{
    [SerializeField] private GameObject _kickFill;
    [SerializeField] private GameObject _snareFill;
    [SerializeField] private GameObject _selection;


    public void SetKickActive(bool isActive)
    {
        _kickFill.SetActive(isActive);
    }

    public void SetSnareActive(bool isActive)
    {
        _snareFill.SetActive(isActive);
    }

    public void SetSelectionActive(bool isActive)
    {
        _selection.SetActive(isActive);
    }

    private void Awake()
    {
        SetSelectionActive(false);
    }
}
