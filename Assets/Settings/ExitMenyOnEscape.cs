using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenyOnEscape : MonoBehaviour
{
    [SerializeField] GameObject landingPage;

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            landingPage.SetActive(true);
            
        }
    }
}
