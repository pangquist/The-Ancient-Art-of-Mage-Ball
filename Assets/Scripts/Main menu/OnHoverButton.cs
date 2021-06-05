using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Simple script that enlarges buttons that have the scripts on them.
    // Author: Valter Lindecrantz

    Vector3 scale;
    Vector3 enlargedScale;

    float scaleAmount = 1.2f;

    void Start()
    {
        scale = transform.localScale;
        enlargedScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = enlargedScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = scale;
    }
}
