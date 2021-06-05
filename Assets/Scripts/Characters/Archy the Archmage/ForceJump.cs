using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceJump : Ability
{
    // This script handels the functioning of Archy the Archmages second ability, Force Jump.
    // It inherits from the ability class.
    // It is responsible for applying force upwards to the character, making it jump higher than an average jump.
    // Author: Valter Lindecrantz.

    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] SpellSlinging spellSlinging;

    [Header("settings")]
    [SerializeField] Sprite abilityIcon;

    [Header("Values")]
    [SerializeField] float cooldown;
    [SerializeField] float forceJumpHeight;

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

        useAbilities.SetOnCooldown(1, cooldown);

        if (spellSlinging.enabled == true)
        {
            spellSlinging.SpellSling(1, 1);
        }
    }

    public override Sprite ReturnIcon()
    {
        return abilityIcon;
    }
}
