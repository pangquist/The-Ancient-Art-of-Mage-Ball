using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnimationControllerGeomancer : NetworkBehaviour
{
    public Animator anim;
    public PlayerMovement playerMovement;

    [SerializeField]
    KeyCode throwBoulderButton;
    [SerializeField]
    string throwBoulderAnimationName;


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

        if (Input.GetKeyDown(throwBoulderButton) && !anim.GetCurrentAnimatorStateInfo(0).IsName(throwBoulderAnimationName))
        {
            anim.Play(throwBoulderAnimationName);
        }
    }
}
