using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenyOnEscape : MonoBehaviour
{
    [SerializeField] GameObject activePage;

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            activePage.SetActive(true);
            
        }
    }
}
