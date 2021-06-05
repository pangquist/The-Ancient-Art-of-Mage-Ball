using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    // Simple script that sets an object to be destroy after a set amount of time from when it was spawned.
    // Author: Valter Lindecrantz.

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
