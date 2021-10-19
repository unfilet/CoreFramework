using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class ScrollbarHandleSizeFix : UIBehaviour
{
    [SerializeField, Range(0,1)] float size = 0;

    private Scrollbar scrollbar;

    protected override void Awake()
    { 
        scrollbar = GetComponent<Scrollbar>();
        UpdateView();
    }

    protected new IEnumerator Start()
    {
        yield return null;
        UpdateView();
    }

    public void UpdateView()
        => scrollbar.size = size;

    
}
