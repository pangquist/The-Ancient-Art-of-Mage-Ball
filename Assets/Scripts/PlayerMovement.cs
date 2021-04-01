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
        RpcMove();
    }
    #endregion

    #region Client

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority)
            return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);


        if (isGrounded && Input.GetKeyDown(jumpButton))
        {
            velocity.y = jumpPower;
        }
        velocity.y += gravity * Time.deltaTime;

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -4;
        }

        move = transform.right * x + transform.forward * z;
        
        CmdMove();
    }

    [ClientRpc]
    void RpcMove()
    {
        controller.Move(move * speed * Time.deltaTime); //X and Z
        controller.Move(velocity * Time.deltaTime); //Y
    }
    #endregion
}
