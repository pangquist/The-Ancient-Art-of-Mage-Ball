using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{

    // This script handles player input for movement and limitations to that movement.
    // During the update, Input is checked for X and Z, and the appropriate movement is calculated.
    // A networking part of this script is responsible for making sure that the movement that happens are happening for all clients.
    // Author: Valter Lindecrantz

    [Header("Script Dependencies")]
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animator;

    [Header("Settings")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] KeyCode jumpButton;

    [Header("Values")]
    [SerializeField] float jumpPower;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float speed = 8;

    [SerializeField] bool matchIsPaused = true;
    float directionX;
    float directionZ;
    float groundDistance = 0.2f;
    Vector3 move;
    public Vector3 velocity;
    public bool isGrounded;

    public float JumpPower { get { return jumpPower; } set { jumpPower = value; } }
    public float Speed { get { return speed; } set { speed = value; } }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        enabled = true;
        GamestateManager.HandleMatchPaused += Pause;
        GamestateManager.HandleMatchUnpaused += Unpause;
    }

    [Client]
    void Pause()
    {
        matchIsPaused = true;
        Debug.Log("MOVEMENT IS PAUSED");
    }

    [Client]
    void Unpause()
    {
        matchIsPaused = false;
        Debug.Log("MOVEMENT IS UNPAUSED");
    }

    #region Server
    // The command send information from the client to the server that the character should move with the parameters that determines movement for X,Y and Z.
    [Command]
    void CmdMove(Vector3 _move, Vector3 _velocity)
    {
        RpcMove(_move, _velocity);
    }
    #endregion

    #region Client

    //During the update, checks for if the character is standing on the ground and calculations for the characters movement is performed.
    [ClientCallback]
    void Update()
    {
        if (!hasAuthority || matchIsPaused)
        {
            return;
        }


        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && Input.GetKeyDown(jumpButton) && !InGameMenu.gameIsPaused)
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

        if (InGameMenu.gameIsPaused)
        {
            move = new Vector3(0, 0, 0);
        }

        if (move != Vector3.zero)
        {
            if (directionX < 0)
            {
                animator.SetBool("isStrafingLeft", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isStrafingRight", false);
            }
            else if (directionX > 0)
            {
                animator.SetBool("isStrafingRight", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isStrafingLeft", false);
            }
            else if (directionX == 0)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isStrafingRight", false);
                animator.SetBool("isStrafingLeft", false);
            }

        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isStrafingLeft", false);
            animator.SetBool("isStrafingRight", false);
        }

        if (NetworkClient.ready)
        {
            CmdMove(move, velocity);
        }
        

    }

    // The ClientRpc sends out the appropriate information to all active clients, telling them to move the character in the given direction.
    [ClientRpc]
    void RpcMove(Vector3 _move, Vector3 _velocity)
    {
        controller.Move(_move * speed * Time.deltaTime); //X and Z
        controller.Move(_velocity * Time.deltaTime); //Y
    }
    
    #endregion
}
