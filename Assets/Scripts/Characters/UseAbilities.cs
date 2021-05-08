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
    [SerializeField] AnimationClip ability1Name;

    [Header("Ability 2")]
    [SerializeField] KeyCode castAbility2Button;
    [SerializeField] float cooldownAbility2;
    float currentAbility2Cooldown;
    [SerializeField] AnimationClip ability2Name;

    [Header("Ability 3")]
    [SerializeField] KeyCode castAbility3Button;
    [SerializeField] float cooldownAbility3;
    float currentAbility3Cooldown;
    [SerializeField] AnimationClip ability3Name;

    [Header("Ability 4")]
    [SerializeField] KeyCode castAbility4Button;
    [SerializeField] float cooldownAbility4;
    float currentAbility4Cooldown;
    [SerializeField] AnimationClip ability4Name;

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
        if (currentAbility3Cooldown > 0)
        {
            currentAbility3Cooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(castAbility1Button) && currentAbility1Cooldown <= 0)
        {
            StartAbility1Animation();
        }

        if (Input.GetKeyDown(castAbility2Button) && currentAbility2Cooldown <= 0)
        {
            StartAbility2Animation();
        }

        if (Input.GetKeyDown(castAbility3Button) && currentAbility3Cooldown <= 0)
        {
            StartAbility3Animation();
        }
    }

    void StartAbility1Animation()
    {
        anim.Play(ability1Name.name);
        currentAbility1Cooldown = cooldownAbility1;
    }

    void StartAbility2Animation()
    {
        anim.Play(ability2Name.name);
        currentAbility2Cooldown = cooldownAbility2;
    }

    void StartAbility3Animation()
    {
        anim.Play(ability3Name.name);
        currentAbility3Cooldown = cooldownAbility3;
    }
}
