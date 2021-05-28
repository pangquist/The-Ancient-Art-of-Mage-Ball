using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] GameObject tooltipBackground;

    [Header("Lists")]
    [SerializeField] List<string> headers;
    [SerializeField] List<string> contents;
    [SerializeField] List<Tooltip> tooltips;
    [SerializeField] List<Sprite> abilityImages;
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
        tooltipBackground.SetActive(true);
        for (int i = 0; i < tooltips.Count; i++)
        {
            tooltips[i].ShowToolTip(tooltipsGO[i], tooltips[i], contents[i], abilityImages[i], headers[i]);
        }
    }
 

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipBackground.SetActive(false);
        for (int i = 0; i < tooltips.Count; i++)
        {
            tooltips[i].HideToolTip(tooltipsGO[i]);
        }
    }
}

