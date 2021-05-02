using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField] float time;

    private void Start()
    {
        DestroyObjectAfterSeconds(time);
    }

    public void DestroyObjectAfterSeconds(float value)
    {
        Destroy(gameObject, value);
    }
}
