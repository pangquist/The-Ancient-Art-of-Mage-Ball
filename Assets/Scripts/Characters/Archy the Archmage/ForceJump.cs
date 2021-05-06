using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceJump : NetworkBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] float forceJumpHeight;

    [Client]
    void DoForceJump()
    {
        playerMovement.velocity.y += forceJumpHeight;
    }
}
