using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSlinging : Passive
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbility;

    [Header("Settings")]
    [SerializeField] Sprite passiveIcon;

    private void Start()
    {
        
    }

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
