using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : NetworkBehaviour
{
    // This script handles special movement for the ball, like defying gravity.
    // During the update, a bool is checked for whether the ball is affected by gravity or not.
    // A networking part of this script is responsible for making sure that the balls gravity is synced between all the clients.
    // Author: Valter Lindecrantz

    Rigidbody rb;
    [SerializeField] GameObject FXEffect;

    float revertedGravityDuration = 0;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }
    
    // During the update, the gravity-bool is checked. If it is turned off, a timer begins that measures how long the ball will be airborne for.
    // When the counter has reached the limit, the bool is turned on, and the ball in turn starts to be affected by gravity again.
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
            FXEffect.SetActive(false);
            revertedGravityDuration = 0;
       }
    }
    
    [ClientRpc]
    public void RevertGravity(float duration)
    {
        rb.useGravity = false;
        revertedGravityDuration = duration;
        FXEffect.SetActive(true);
    }
}
