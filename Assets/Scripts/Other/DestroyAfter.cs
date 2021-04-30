using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public void DestroyObjectAfterSeconds(float value)
    {
        Destroy(gameObject, value);
    }
}
