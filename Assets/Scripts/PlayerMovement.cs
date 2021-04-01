using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    
    public CharacterController controller;
    [SerializeField]
    float speed = 8;
    [SerializeField]
    Transform groundCheck;
    float groundDistance = 0.4f;
    [SerializeField]
    LayerMask groundMask;

    public bool isGrounded;

    [SerializeField]
    KeyCode jumpButton;
    [SerializeField]
    float jumpPower;

    float x;
    float z;
    [SerializeField]
    float gravity = -9.81f;

    Vector3 move;
    public Vector3 velocity;  

    #region Server
    [Command]
    void CmdMove()
    {
        
    }
    #endregion

    #region Client
    
    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority)
            return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }

        if (isGrounded && Input.GetKeyDown(jumpButton))
        {
            velocity.y = jumpPower;
        }

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        move = transform.right * x + transform.forward * z;
        velocity.y += gravity * Time.deltaTime;


        //CmdMove();
        controller.Move(move * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
    }
    #endregion
}
