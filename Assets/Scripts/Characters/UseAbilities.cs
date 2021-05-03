using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAbilities : NetworkBehaviour
{
    Animator anim;

    [Header("Ability 1")]
    [SerializeField] KeyCode castAbility1Button;
    [SerializeField] float cooldownAbility1; //Stop the hardcode
    float currentAbility1Cooldown;
    [SerializeField] string ability1Name;

    [Header("Ability 2")]
    [SerializeField] KeyCode castAbility2Button;
    [SerializeField] float cooldownAbility2;
    float currentAbility2Cooldown;
    [SerializeField] string ability2Name;

    [Header("Ability 3")]
    [SerializeField] KeyCode castAbility3Button;
    [SerializeField] float cooldownAbility3;
    float currentAbility3Cooldown;
    [SerializeField] string ability3Name;

    [Header("Ability 4")]
    [SerializeField] KeyCode castAbility4Button;
    [SerializeField] float cooldownAbility4;
    float currentAbility4Cooldown;
    [SerializeField] string ability4Name;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnStartAuthority()
    {
        enabled = true;

        currentAbility1Cooldown = 0;
        currentAbility2Cooldown = 0;
        currentAbility3Cooldown = 0;
        currentAbility4Cooldown = 0;
    }

    private void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        if (currentAbility1Cooldown > 0)
        {
            currentAbility1Cooldown -= Time.deltaTime;
        }

        if (currentAbility2Cooldown > 0)
        {
            currentAbility2Cooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(castAbility1Button) && currentAbility1Cooldown <= 0)
        {
            StartAbility1Animation();
        }

        if (Input.GetKeyDown(castAbility2Button) && currentAbility2Cooldown <= 0)
        {
            StartAbility2Animation();
        }
    }

    void StartAbility1Animation()
    {
        anim.Play(ability1Name);
        currentAbility1Cooldown = cooldownAbility1;
    }

    void StartAbility2Animation()
    {
        anim.Play(ability2Name);
        currentAbility2Cooldown = cooldownAbility2;
    }
}
