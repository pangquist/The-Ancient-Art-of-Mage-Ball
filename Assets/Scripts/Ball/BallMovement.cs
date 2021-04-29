using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    Rigidbody rb;

    float revertedGravityDuration = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.useGravity == true)
        {
            return;
        }

       if (revertedGravityDuration > 0)
       {
           revertedGravityDuration -= Time.deltaTime;
       }

       if (revertedGravityDuration <= 0)
       {
            rb.useGravity = true;
            revertedGravityDuration = 0;
       }
    }

    public void RevertGravity(float duration)
    {
        rb.useGravity = false;
        revertedGravityDuration = duration;
    }
}
