using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackground : MonoBehaviour
{
    public Material[] skyboxes;
    [SerializeField] GameObject light;

    // Use this for initialization
    void Start()
    {
        ChangeMySkybox();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void ChangeMySkybox()
    {
        int x = Random.Range(0, skyboxes.Length - 1);
        RenderSettings.skybox = skyboxes[x];

        if (x == 0)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + 31f,
           transform.localEulerAngles.y + 124f, transform.localEulerAngles.z);
        }

        if (x == 1)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + 25f,
           transform.localEulerAngles.y + 185f, transform.localEulerAngles.z + 3f);
        }

        if (x == 2)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + 25f,
           transform.localEulerAngles.y + 185f, transform.localEulerAngles.z + 3f);
        }

        if (x == 3)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + 30f,
           transform.localEulerAngles.y + 175.5f, transform.localEulerAngles.z + (-9f));
        }
    }
}
