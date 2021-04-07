using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : NetworkBehaviour
{
    public Animator anim;
    public PlayerMovement playerMovement;

    [SerializeField]
    KeyCode castButton;
    [SerializeField]
    string castAnimationName;

    [SerializeField]
    KeyCode jumpButton;
    [SerializeField]
    string jumpAnimationName;

    [SerializeField]
    KeyCode dashButton;
    [SerializeField]
    string dashAnimationName;
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    //Reads the input and activates the corresponding animation on the character prefab.
    [ClientCallback]
    void Update()
    {
        if (!hasAuthority)
            return;

        if (Input.GetKeyDown(castButton) && !anim.GetCurrentAnimatorStateInfo(0).IsName(castAnimationName))
        {
            anim.Play(castAnimationName);
        }
        if (Input.GetKeyDown(jumpButton) && playerMovement.isGrounded)
        {
            anim.Play(jumpAnimationName);
        }
        if (Input.GetKeyDown(dashButton) && ForceDash.currentNumberOfCharges != 0)
        {
            anim.Play(dashAnimationName);
            ForceDash.currentNumberOfCharges--;
        }
    }
}
