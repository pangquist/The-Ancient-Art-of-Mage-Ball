using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : NetworkBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    [SerializeField] LayerMask grappleable;
    [SerializeField] Transform castPoint, camera, player;
    private float maxRange = 25f;

    private RaycastHit raycastHit;

    PlayerMovement playerMovement;
    [SerializeField]
    float grappleSpeed;

    [SerializeField]
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
    }

    private void Update()
    {
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
            grapplePoint = raycastHit.transform.position;

            Vector3 grappleVector = player.position - grapplePoint;

            Rigidbody ball = raycastHit.transform.gameObject.GetComponent<Rigidbody>();


            ball.AddForce(grappleVector * grappleSpeed * 10 * Time.deltaTime);
        }
        else
        {
            Vector3 grappleVector = grapplePoint - player.position;

            playerMovement.controller.Move(grappleVector * grappleSpeed * 2 * Time.deltaTime);
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
        lr.positionCount = 0;
    }
}
