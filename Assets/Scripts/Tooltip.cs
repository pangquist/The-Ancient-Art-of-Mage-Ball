using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    
    public static Tooltip instance;
    public static List<GameObject> tooltips;
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterLimit;

    private void Awake()
    {
        instance = this;
    }

    public void ShowToolTip(GameObject tooltipGO,Tooltip tooltip, string content, string header = "")
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
        
        
    }

    public void HideToolTip(GameObject tooltip)
    {
        tooltip.SetActive(false);
    }
    


    private void Update()
    {
        if (Application.isEditor)
        {
            int headerLength = headerField.text.Length;
            int contentLength = contentField.text.Length;

            if (headerLength > characterLimit || contentLength > characterLimit)
                layoutElement.enabled = true;
            else
                layoutElement.enabled = false;
        }

    }
}
