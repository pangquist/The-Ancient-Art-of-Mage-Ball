using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAbilities : NetworkBehaviour
{
    // This script handles all the general things related to the numbers of the cooldowns of each character.
    // This script enables the programmer to set a Key, cooldown and ability to play for each ability slot.
    // During the update, the cooldowns will gradually decrease if they are above 0, and the script checks for keycode input from the player.
    // When the appropriate key is pressed, the current cooldown will be set to max and the animation will be player.
    // Author: Valter Lindecrantz

    [Header("Settings")]
    [SerializeField] Animator anim;

    #region Ability1
    [Header("Ability 1")]
    [SerializeField] KeyCode castAbility1Button;
    [SerializeField] float cooldownAbility1;
    [SerializeField] AnimationClip ability1Name;
    public float CooldownAbility1() { return cooldownAbility1; }
    #endregion

    #region Ability2
    [Header("Ability 2")]
    [SerializeField] KeyCode castAbility2Button;
    [SerializeField] float cooldownAbility2;
    [SerializeField] AnimationClip ability2Name;
    public float CooldownAbility2() { return cooldownAbility2; }
    #endregion

    #region Ability3
    [Header("Ability 3")]
    [SerializeField] KeyCode castAbility3Button;
    [SerializeField] float cooldownAbility3;
    [SerializeField] AnimationClip ability3Name;
    public float CooldownAbility3() { return cooldownAbility3; }
    #endregion

    float[] currentCooldowns;
    float[] cooldowns;
    [SerializeField] bool matchIsPaused = true;

    // Getter for the Cooldown UI to retrieve the current cooldowns to correctly display on the UI.
    public float GetCooldown(int cooldownIndex) 
    {
        return currentCooldowns[cooldownIndex];
    }

    // During start the character retrieves its own animator component to be able to reference the animation clips.
    void Start()
    {
        currentCooldowns = new float[3];
        for (int i = 0; i< currentCooldowns.Length; i++)
        {
            currentCooldowns[i] = 0f;
        }

        cooldowns = new float[3];
        for (int i = 0; i < cooldowns.Length; i++)
        {
            if (i == 0)
            {
                cooldowns[i] = cooldownAbility1;
            }
            else if (i == 1)
            {
                cooldowns[i] = cooldownAbility2;
            }
            else if (i == 2)
            {
                cooldowns[i] = cooldownAbility3;
            }
        }
    }

    public override void OnStartAuthority()
    { 
        base.OnStartAuthority();
        GamestateManager.HandleMatchPaused += Pause;
        GamestateManager.HandleMatchUnpaused += Unpause;
    }

    [Client]
    void Pause()
    {
        matchIsPaused = true;
        Debug.Log("ABILITIES IS PAUSED");
    }

    [Client]
    void Unpause()
    {
        matchIsPaused = false;
        Debug.Log("ABILITIES IS UNPAUSED");
    }

    private void Update()
    {
        if (!hasAuthority || matchIsPaused)
        {
            return;
        }

        // If a cooldown is currently above 0, it's reduced over time until hitting 0.
        for (int i = 0; i < currentCooldowns.Length; i++)
        {
            if (currentCooldowns[i] > 0)
            {
                currentCooldowns[i] -= Time.deltaTime;
            }
        }

        // If the correct key is pressed and the cooldown is less then or equal to 0, calls on the method that starts the appropriate animation.
        if(!InGameMenu.gameIsPaused)
        {
            if (Input.GetKeyDown(castAbility1Button) && currentCooldowns[0] <= 0 /*&& anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")*/)
            {
                StartAbility1Animation();
            }

            if (Input.GetKeyDown(castAbility2Button) && currentCooldowns[1] <= 0 /*&& anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")*/)
            {
                StartAbility2Animation();
            }

            if (Input.GetKeyDown(castAbility3Button) && currentCooldowns[2] <= 0 /*&& anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")*/)
            {
                StartAbility3Animation();
            }
        }
    }

    // Plays the appropriate animation (which contains the method for the ability) and sets the cooldown to max.
    void StartAbility1Animation()
    {
        anim.Play(ability1Name.name);
    }

    void StartAbility2Animation()
    {
        anim.Play(ability2Name.name);
    }

    void StartAbility3Animation()
    {
        anim.Play(ability3Name.name);
    }

    public void SetOnCooldown(int abilityIndex, float cooldownAmount)
    {
        currentCooldowns[abilityIndex] = cooldownAmount;
    }

    public void ReduceAllCooldowns(float reduceAmount, int attackIndex)
    {
        for (int i = 0; i < currentCooldowns.Length; i++)
        {
            if (currentCooldowns[i] > 0 && i != attackIndex)
            {
                currentCooldowns[i] -= reduceAmount;
            }
        }
    }

    public void ResetAllCooldowns()
    {
        for (int i = 0; i < currentCooldowns.Length; i++)
        {
           currentCooldowns[i] = 0;
        }
    }

    public void StartAllCooldowns()
    {
        for (int i = 0; i < currentCooldowns.Length; i++)
        {
            currentCooldowns[i] = cooldowns[i];
        }
    }

    public void SetCooldownToPercentage(int cooldownIndex, float cooldownAmount, float percentage)
    {
        currentCooldowns[cooldownIndex] = cooldownAmount * (percentage/100);
    }
}
