using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement1 : NetworkBehaviour
{
<<<<<<< HEAD
    
    public CharacterController controller;
    [SerializeField]
    public float speed = 8;
    [SerializeField]
    Transform groundCheck;
    float groundDistance = 0.2f;
    [SerializeField]
    LayerMask groundMask;

    float timer;

    public bool isGrounded;

    bool playingAnim = false;

    [SerializeField]
    KeyCode jumpButton;
    [SerializeField]
    float jumpPower;
    Animator anim;
    float x;
    float z;
    [SerializeField]
    float gravity = -9.81f;

    public Vector3 move;
    public Vector3 velocity;

    private void Start()
    {
        anim = GetComponent<Animator>();
=======
    // This script handles player input for movement and limitations to that movement.
    // During the update, Input is checked for X and Z, and the appropriate movement is calculated.
    // A networking part of this script is responsible for making sure that the movement that happens are happening for all clients.
    // Author: Valter Lindecrantz

    [SerializeField] CharacterController controller;
    [SerializeField] Animator animator;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] KeyCode jumpButton;
    [SerializeField] float jumpPower;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float speed = 8;

    [SerializeField] bool matchIsPaused = true;
    float directionX;
    float directionZ;
    float groundDistance = 0.6f;
    Vector3 move;
    public Vector3 velocity;
    public bool isGrounded;
    
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        enabled = true;
        GamestateManager.HandleMatchPaused += TogglePause;
        Debug.Log("MOVEMENT IS SUBSCRIBED!");
    }

    void TogglePause()
    {
        if (matchIsPaused)
        {
           matchIsPaused = false;
        }
        else if (!matchIsPaused)
        {
            matchIsPaused = true;
        }
>>>>>>> main
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
<<<<<<< HEAD
        
=======
        }

>>>>>>> main
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

<<<<<<< HEAD
        //timer += Time.deltaTime;

        //if (move.x != 0 || move.z != 0)
        //{
        //    Debug.Log("Inside");
        //    if (timer> runningAnimation.length)
        //    {
        //        anim.Play(runningAnimation.name);
        //        timer = 0;                
        //    }            
        //}

        move = transform.right * x + transform.forward * z;

        if (move.x != 0 || move.z != 0)
        {
            Running();
        }
        else
        {
            Idle();
        }


            CmdMove(move, velocity);
=======
        move = transform.right * directionX + transform.forward * directionZ;

        if (InGameMenu.gameIsPaused)
        {
            move = new Vector3(0, 0, 0);
        }

        if (move != Vector3.zero)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        CmdMove(move, velocity);
>>>>>>> main
    }

    // The ClientRpc sends out the appropriate information to all active clients, telling them to move the character in the given direction.
    [ClientRpc]
    void RpcMove(Vector3 _move, Vector3 _velocity)
    {
        controller.Move(_move * speed * Time.deltaTime); //X and Z
        controller.Move(_velocity * Time.deltaTime); //Y
    }
    [Client]
    void Idle()
    {
        anim.SetFloat("Speed", 0);
    }

    [Client]
    void Running()
    {
        anim.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
    }
    #endregion
}
