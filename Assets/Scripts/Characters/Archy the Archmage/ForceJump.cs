using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceJump : NetworkBehaviour
{
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] float forceJumpHeight;

    [Client]
    void DoForceJump()
    {
        playerMovement.velocity.y += forceJumpHeight;

        useAbilities.SetOnCooldownAbility2();
    }
}
