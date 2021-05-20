using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceJump : NetworkBehaviour
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] PlayerMovement playerMovement;

    [Header("Values")]
    [SerializeField] float forceJumpHeight;

    [Client]
    void DoForceJump()
    {
        playerMovement.velocity.y += forceJumpHeight;

        useAbilities.SetOnCooldownAbility2();
        useAbilities.ReduceAllCooldowns(1, 1);
    }
}
