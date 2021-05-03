using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFrameRate : MonoBehaviour
{
    [SerializeField] int frameRateCap = 144;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = frameRateCap;
    }
}
