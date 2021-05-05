using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] KeyCode jumpButton;
    [SerializeField] float jumpPower;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float speed = 8;

    float directionX;
    float directionZ;
    float groundDistance = 0.2f;
    Vector3 move;
    public Vector3 velocity;
    public bool isGrounded;

    #region Server
    [Command]
    void CmdMove(Vector3 _move, Vector3 _velocity)
    {
        RpcMove(_move, _velocity);
    }
    #endregion

    #region Client

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && Input.GetKeyDown(jumpButton))
        {
            velocity.y = jumpPower;
        }

        velocity.y += gravity * Time.deltaTime;

        directionX = Input.GetAxis("Horizontal");
        directionZ = Input.GetAxis("Vertical");

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }

        move = transform.right * directionX + transform.forward * directionZ;
        
        CmdMove(move, velocity);
    }

    [ClientRpc]
    void RpcMove(Vector3 _move, Vector3 _velocity)
    {
        controller.Move(_move * speed * Time.deltaTime); //X and Z
        controller.Move(_velocity * Time.deltaTime); //Y
    }
    #endregion
}
