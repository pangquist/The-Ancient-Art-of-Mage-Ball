using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSlinging : Passive
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbility;

    private void Start()
    {
        
    }

    public void SpellSling(float cooldownReduction, int abilityIndex)
    {
        useAbility.ReduceAllCooldowns(cooldownReduction, abilityIndex);
    }

}
