using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSettings : MonoBehaviour
{
    [SerializeField] Settings settings;

    private void Start()
    {
        settings.LoadSettings();
    }
}
