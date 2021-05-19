using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 scale;
    Vector3 enlargedScale;

    float scaleAmount = 1.2f;

    [SerializeField] AudioSource sound;

    void Start()
    {
        scale = transform.localScale;
        enlargedScale = new Vector3(scaleAmount, scaleAmount, scaleAmount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = enlargedScale;

        if (sound != null)
        {
            sound.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = scale;
    }
}
