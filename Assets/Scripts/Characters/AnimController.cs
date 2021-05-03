using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : NetworkBehaviour
{
    public Animator anim;
    public PlayerMovement playerMovement;

    [SerializeField]
    KeyCode ability1Button;
    [SerializeField]
    string ability1Name;

    [SerializeField]
    KeyCode ability2Button;
    [SerializeField]
    string ability2Name;

    [SerializeField]
    KeyCode ability3Button;
    [SerializeField]
    string ability3Name;
    
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

        if (Input.GetKeyDown(ability1Button) && !anim.GetCurrentAnimatorStateInfo(0).IsName(ability1Name))
        {
            anim.Play(ability1Name);
        }
        if (Input.GetKeyDown(ability2Button) && !anim.GetCurrentAnimatorStateInfo(0).IsName(ability2Name)/* && playerMovement.isGrounded*/)
        {
            anim.Play(ability2Name);
        }
        if (Input.GetKeyDown(ability3Button) && !anim.GetCurrentAnimatorStateInfo(0).IsName(ability3Name)/* && ForceDash.currentNumberOfCharges != 0*/)
        {
            anim.Play(ability3Name);
            //ForceDash.currentNumberOfCharges--;
        }
    }
}
