using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSlinging : Passive
{
    // This script handles the functioning of Archy's spellslinging passive.
    // It inherits from the Passive class.
    // Every time archy uses an ability, this script makes it so that every other abilities cooldown is reduced.
    // Author: Valter Lindecrantz.

    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbility;

    [Header("Settings")]
    [SerializeField] Sprite passiveIcon;

    private void Start()
    {
        
    }

    // When one of Archys abilities is used, it calls this function and sends in the parameter of how much the other abilities cooldowns will be reduced.
    public void SpellSling(float cooldownReduction, int abilityIndex)
    {
        if (enabled == true)
        {
            useAbility.ReduceAllCooldowns(cooldownReduction, abilityIndex);
        }
    }

    public override Sprite ReturnIcon()
    {
        return passiveIcon;
    }

}
