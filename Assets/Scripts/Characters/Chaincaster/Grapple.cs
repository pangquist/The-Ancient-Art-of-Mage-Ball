using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : NetworkBehaviour
{
    private LineRenderer lr;
    private Material material;
    private Vector2 offsetSpeed = new Vector2(15, 0);

    private Vector3 grapplePoint;
    [SerializeField] LayerMask grappleable;
    [SerializeField] Transform castPoint, camera, player;
    private float maxRange = 25f;

    private RaycastHit raycastHit;

    PlayerMovement playerMovement;
    [SerializeField]
    float grappleSpeed;

    [SerializeField]
    float duration;
    float currentDuration;
    float cooldown;
    float timeSinceLastCast;
    [SerializeField]
    bool isOnCooldown;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();

        lr.positionCount = 0;

        material = lr.material;

        currentDuration = duration;
    }

    //[Client]
    private void Update()
    {
        //if (!hasAuthority)
        //{
        //    return;
        //}

        material.mainTextureOffset += offsetSpeed * Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (lr.positionCount == 0) return;

        if(raycastHit.transform.gameObject.tag == "Enemy")
        {
            currentDuration -= Time.deltaTime;

            if(currentDuration <= 0)
            {
                StopGrapple();
            }

            grapplePoint = raycastHit.transform.position;

            Vector3 grappleVector = player.position - grapplePoint;

            Rigidbody ball = raycastHit.transform.gameObject.GetComponent<Rigidbody>();

            ball.AddForce(grappleVector * grappleSpeed * 10 * Time.deltaTime);

            //CmdDragBall();
        }
        else
        {
            Vector3 grappleVector = grapplePoint - player.position;

            playerMovement.controller.Move(grappleVector * grappleSpeed * Time.deltaTime);
        }
        
    }

    private void LateUpdate()
    {
        DrawChain();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(camera.position, camera.forward, out hit, maxRange))
        {
            grapplePoint = hit.point;

            lr.positionCount = 2;

            raycastHit = hit;
        }
    }

    void DrawChain()
    {
        if (lr.positionCount == 0) return;

        lr.SetPosition(0, castPoint.position);
        lr.SetPosition(1, grapplePoint);
    }


    void StopGrapple()
    {
        currentDuration = duration;
        lr.positionCount = 0;
    }

    [Command]
    void CmdDragBall()
    {
        grapplePoint = raycastHit.transform.position;

        Vector3 grappleVector = player.position - grapplePoint;

        Rigidbody ball = raycastHit.transform.gameObject.GetComponent<Rigidbody>();

        ball.AddForce(grappleVector * grappleSpeed * 10 * Time.deltaTime);
    }
}
