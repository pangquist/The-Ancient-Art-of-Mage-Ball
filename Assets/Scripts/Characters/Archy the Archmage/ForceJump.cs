using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceJump : Ability
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] PlayerMovement playerMovement;

    [Header("Values")]
    [SerializeField] float forceJumpHeight;

    private void OnEnable()
    {
        useAbilities = GetComponent<UseAbilities>();
        playerMovement = GetComponent<PlayerMovement>();

        forceJumpHeight = 15;
    }

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [Client]
    public override void UseAbility(int abilityIndex)
    {
        if (abilityIndex != 2)
        {
            return;
        }

        playerMovement.velocity.y += forceJumpHeight;

        useAbilities.SetOnCooldown(1);
        useAbilities.ReduceAllCooldowns(1, 1);
    }
}
