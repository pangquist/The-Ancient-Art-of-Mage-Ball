using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    
    [SerializeField] List<string> headers;
    [SerializeField] List<string> contents;
    [SerializeField] List<Tooltip> tooltips;
    [SerializeField] List<GameObject> tooltipsGO;

    public void Awake()
    {
        for (int i = 0; i < tooltips.Count; i++)
        {
            tooltips[i].HideToolTip(tooltipsGO[i]);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        for (int i = 0; i < tooltips.Count; i++)
        {
            
            tooltips[i].ShowToolTip(tooltipsGO[i], tooltips[i], contents[i], headers[i]);
        }
    }
 

    public void OnPointerExit(PointerEventData eventData)
    {
        for (int i = 0; i < tooltips.Count; i++)
        {
            tooltips[i].HideToolTip(tooltipsGO[i]);
        }
    }
}

