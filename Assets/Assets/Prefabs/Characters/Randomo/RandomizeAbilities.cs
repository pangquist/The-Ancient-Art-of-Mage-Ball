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

    [Header("Lists")]
    [SerializeField] List<string> firstAbilities;
    [SerializeField] List<string> secondAbilities;
    [SerializeField] List<string> thirdAbilities;
    [SerializeField] List<string> fourthAbilities;

    [Header("Abilities")]
    [SerializeField] Ability ability1;
    [SerializeField] Ability ability2;
    [SerializeField] Ability ability3;
    [SerializeField] string ability4;

    [Header("Ability Icons")]
    [SerializeField] Image abilityIcon1;
    [SerializeField] Image abilityIcon2;
    [SerializeField] Image abilityIcon3;
    
    System.Random random = new System.Random();

    void Start()
    {
        allAbilities = GetComponents<Ability>();
        
        Type ability1Type = Type.GetType(firstAbilities[random.Next(0, firstAbilities.Count)]);
        Type ability2Type = Type.GetType(secondAbilities[random.Next(0, secondAbilities.Count)]);
        Type ability3Type = Type.GetType(thirdAbilities[random.Next(0, thirdAbilities.Count)]);
        
        foreach (Ability ability in allAbilities)
        {
            if (ability.GetType() == ability1Type || ability.GetType() == ability2Type || ability.GetType() == ability3Type)
            {
                continue;
            }
            ability.enabled = false;
        }

        ability1 = (Ability)gameObject.GetComponent(ability1Type);
        ability2 = (Ability)gameObject.GetComponent(ability2Type);
        ability3 = (Ability)gameObject.GetComponent(ability3Type);
        ability4 = fourthAbilities[random.Next(0, fourthAbilities.Count)];

        abilityIcon1.sprite = ability1.ReturnIcon();
        abilityIcon2.sprite = ability2.ReturnIcon();
        abilityIcon3.sprite = ability3.ReturnIcon();
    }

    public void UseAbility1()
    {
        Debug.Log("Using Ability 1");
        ability1.UseAbility(1);
    }

    public void UseAbility2()
    {
        Debug.Log("Using Ability 2");
        ability2.UseAbility(2);
    }

    public void UseAbility3()
    {
        Debug.Log("Using Ability 3");
        ability3.UseAbility(3);
    }

}
