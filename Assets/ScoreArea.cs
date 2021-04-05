using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
       if (other.GetComponent<GameObject>() != null)
        {
            Debug.Log("");
        }
    }
}
