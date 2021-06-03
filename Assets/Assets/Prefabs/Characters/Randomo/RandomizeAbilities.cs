using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeAbilities : MonoBehaviour
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;

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

    System.Random random = new System.Random();

    void Start()
    {
        Type ability1Type = Type.GetType(firstAbilities[random.Next(0, firstAbilities.Count)]);
        Type ability2Type = Type.GetType(secondAbilities[random.Next(0, secondAbilities.Count)]);
        Type ability3Type = Type.GetType(thirdAbilities[random.Next(0, thirdAbilities.Count)]);


        ability1 = (Ability)gameObject.GetComponent(ability1Type);
        ability2 = (Ability)gameObject.GetComponent(ability2Type);
        ability3 = (Ability)gameObject.GetComponent(ability3Type);
        ability4 = fourthAbilities[random.Next(0, fourthAbilities.Count)];
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
