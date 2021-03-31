using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerashake : MonoBehaviour
{
    public  GameObject  Camera; // set this via inspector
  public float shake = 0f;
  public float shakeAmount  = 0.2f;
   public float decreaseFactor = 1.0f;
 
   void  Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))                                              //  (shake > 0)
        {
            Camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
            shake -= Time.deltaTime * decreaseFactor;

        }
        else
        {
            shake = 0.0f;
        }
    }
}
