using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomizeAbilities : MonoBehaviour
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;

    [Header("Available Abilities")]
    [SerializeField] Ability[] allAbilities;

    [Header("Available Passive")]
    [SerializeField] Passive[] allPassives;

    [Header("Lists")]
    [SerializeField] List<Ability> firstAbilities;
    [SerializeField] List<Ability> secondAbilities;
    [SerializeField] List<Ability> thirdAbilities;
    [SerializeField] List<Passive> passives;

    [Header("Abilities")]
    [SerializeField] Ability randomizedAbility1;
    [SerializeField] Ability randomizedAbility2;
    [SerializeField] Ability randomizedAbility3;

    [Header("Passive")]
    [SerializeField] Passive randomizedPassive;

    [Header("Ability Icons")]
    [SerializeField] Image abilityIcon1;
    [SerializeField] Image abilityIcon2;
    [SerializeField] Image abilityIcon3;
    [SerializeField] Image passiveIcon;
    
    System.Random random = new System.Random();

    void Start()
    {
        allAbilities = GetComponents<Ability>();
        allPassives = GetComponents<Passive>();

        randomizedAbility1 = firstAbilities[random.Next(0, firstAbilities.Count)];
        randomizedAbility2 = secondAbilities[random.Next(0, secondAbilities.Count)];
        randomizedAbility3 = thirdAbilities[random.Next(0, thirdAbilities.Count)];

        foreach (Ability ability in allAbilities)
        {
            if (ability == randomizedAbility1)
            {
                ability.enabled = true;
            }
            else if(ability == randomizedAbility2)
            {
                ability.enabled = true;
            }
            else if(ability == randomizedAbility3)
            {
                ability.enabled = true;
            }
            else
            {
                ability.enabled = false;
            }
        }

        randomizedPassive = allPassives[random.Next(0, passives.Count)];

        foreach (Passive passive in allPassives)
        {
            if(passive != randomizedPassive)
            {
                passive.enabled = false;
            }
        }
        
        abilityIcon1.sprite = randomizedAbility1.ReturnIcon();
        abilityIcon2.sprite = randomizedAbility2.ReturnIcon();
        abilityIcon3.sprite = randomizedAbility3.ReturnIcon();
        passiveIcon.sprite = randomizedPassive.ReturnIcon();
    }

    [Client]
    public void UseAbility1()
    {
        Debug.Log("Using Ability 1");
        randomizedAbility1.UseAbility(1);
    }

    [Client]
    public void UseAbility2()
    {
        Debug.Log("Using Ability 2");
        randomizedAbility2.UseAbility(2);
    }

    [Client]
    public void UseAbility3()
    {
        Debug.Log("Using Ability 3");
        randomizedAbility3.UseAbility(3);
    }

}
