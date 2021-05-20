using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    // This script handles all the graphical components of the cooldown UI system.
    // During the update, the GUI will update to show the current cooldown of the ability which it is connected to.
    // Author: Valter Lindecrantz

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

    [SerializeField] bool matchIsPaused = true;

    private void Start()
    {
        GamestateManager.HandleMatchPaused += TogglePause;
    }

    void TogglePause()
    {
        if (matchIsPaused)
        {
            matchIsPaused = false;
        }
        else if (!matchIsPaused)
        {
            matchIsPaused = true;
        }
    }

    // During the update, callculations are performed to make the UI accurately display the current cooldown of the given ability.
    void Update()
    {
        if (matchIsPaused)
        {
            return;
        }

        #region Ability1
        if (abilities.GetCooldown(0) <= 0)
        {
            ability1Icon.fillAmount = 0;
            ability1Text.text = "";
        }
        else
        {
            ability1Icon.fillAmount = abilities.GetCooldown(0) / abilities.CooldownAbility1();
            ability1Text.text = (abilities.GetCooldown(0)).ToString(".#");
        }
        #endregion

        #region Ability2
        if (abilities.GetCooldown(1) <= 0)
        {
            ability2Icon.fillAmount = 0;
            ability2Text.text = "";
        }
        else
        {
            ability2Icon.fillAmount = abilities.GetCooldown(1) / abilities.CooldownAbility2();
            ability2Text.text = Convert.ToInt32(abilities.GetCooldown(1)).ToString();
        }
        #endregion

        #region Ability3
        if (abilities.GetCooldown(2) <= 0)
        {
            ability3Icon.fillAmount = 0;
            ability3Text.text = "";
        }
        else
        {
            ability3Icon.fillAmount = abilities.GetCooldown(2) / abilities.CooldownAbility3();
            ability3Text.text = Convert.ToInt32(abilities.GetCooldown(2)).ToString();
        }
        #endregion
    }
}
