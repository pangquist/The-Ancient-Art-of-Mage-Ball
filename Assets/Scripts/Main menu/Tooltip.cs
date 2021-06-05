using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    //This script sets the tooltips active and changes the content depending on what character you are hovering over at the moment
    //Author Emil Påsse

    public static Tooltip instance;
    public static List<GameObject> tooltips;
    public TextMeshProUGUI headerField;
    public Image abilityImage;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterLimit;

    private void Awake()
    {
        instance = this;
    }

    public void ShowToolTip(GameObject tooltipGO,Tooltip tooltip, string content, Sprite abilityImage, string header = "")
    {
        tooltipGO.SetActive(true);
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
        }
        
        tooltip.contentField.text = content;
        tooltip.headerField.text = header;
        tooltip.abilityImage.sprite = abilityImage;
    }

    public void ShowCharacterName(GameObject tooltipGO, Tooltip tooltip, string header = "")
    {
        tooltipGO.SetActive(true);
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
        }
        tooltip.headerField.text = header;
    }

    public void HideToolTip(GameObject tooltip)
    {
        tooltip.SetActive(false);
    }
    
    private void Update()
    {
        layoutElement.enabled = true;
    }
}
