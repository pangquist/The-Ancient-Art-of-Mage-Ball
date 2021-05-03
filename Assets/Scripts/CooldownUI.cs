using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [Tooltip("Drag in the Character GameObject")]
    [SerializeField] UseAbilities abilities;

    [Header("Ability 1")]
    [SerializeField] Image ability1Icon;
    [SerializeField] TMP_Text ability1Text;

    [Header("Ability 2")]
    [SerializeField] Image ability2Icon;
    [SerializeField] TMP_Text ability2Text;

    [Header("Ability 3")]
    [SerializeField] Image ability3Icon;
    [SerializeField] TMP_Text ability3Text;

    [Header("Ability 4")]
    [SerializeField] Image ability4Icon;
    [SerializeField] TMP_Text ability4Text;

    
    // Update is called once per frame
    void Update()
    {

        //Ability 1
        if (abilities.CurrentAbility1Cooldown() <= 0)
        {
            ability1Icon.fillAmount = 0;
            ability1Text.text = "";
        }
        else
        {
            ability1Icon.fillAmount = (abilities.CurrentAbility1Cooldown() / abilities.CooldownAbility1());
            ability1Text.text = (abilities.CurrentAbility1Cooldown()).ToString(".#");
        }

        //Ability 2
        if (abilities.CurrentAbility2Cooldown() <= 0)
        {
            ability2Icon.fillAmount = 0;
            ability2Text.text = "";
        }
        else
        {
            ability2Icon.fillAmount = abilities.CurrentAbility2Cooldown() / abilities.CooldownAbility2();
            ability2Text.text = Convert.ToInt32(abilities.CurrentAbility2Cooldown()).ToString();
        }

        //Ability 3
        if (abilities.CurrentAbility3Cooldown() <= 0)
        {
            ability3Icon.fillAmount = 0;
            ability3Text.text = "";
        }
        else
        {
            ability3Icon.fillAmount = abilities.CurrentAbility3Cooldown() / abilities.CooldownAbility3();
            ability3Text.text = Convert.ToInt32(abilities.CurrentAbility3Cooldown()).ToString();
        }

        //Ability 4
        if (abilities.CurrentAbility4Cooldown() <= 0)
        {
            ability4Icon.fillAmount = 0;
            ability4Text.text = "";
        }
        else
        {
            ability4Icon.fillAmount = abilities.CurrentAbility4Cooldown() / abilities.CooldownAbility4();
            ability4Text.text = Convert.ToInt32(abilities.CurrentAbility4Cooldown()).ToString();
        }
    }
}
