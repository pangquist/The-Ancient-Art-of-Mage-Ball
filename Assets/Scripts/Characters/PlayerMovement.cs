using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    
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
    }

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
            velocity.y = -2;
        }

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
    }

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
