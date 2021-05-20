using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainGrapple : NetworkBehaviour
{
    private LineRenderer lr;
    private Material material;
    private Vector2 offsetSpeed = new Vector2(15, 0);
    private RaycastHit raycastHit;
    private Vector3 grapplePoint;

    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask grappleable;
    [SerializeField] Transform castPoint, camera, player;
    [SerializeField] private float maxRange = 25f;
    [SerializeField] float grappleSpeed;
    [SerializeField] float duration;
    [SerializeField] float currentDuration;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();

        lr.positionCount = 0;

        material = lr.material;

        currentDuration = duration;
    }

    private void Update()
    {
        material.mainTextureOffset += offsetSpeed * Time.deltaTime;

        if (Input.GetMouseButton(0) == false && currentDuration <= 0)
        {
            StopGrapple();
        }

        if (lr.positionCount == 0) return;

        currentDuration -= Time.deltaTime;

        if (raycastHit.transform.gameObject.tag == "Enemy")
        {
            if(currentDuration <= 0)
            {
                StopGrapple();
            }

            DragBall(raycastHit.transform.gameObject);
        }
        else
        {
            Vector3 grappleVector = grapplePoint - player.position;

            controller.Move(grappleVector * grappleSpeed * Time.deltaTime);
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

    [Client]
    void DragBall(GameObject target)
    {
        if (!hasAuthority)
        {
            return;
        }

        grapplePoint = target.transform.position;

        Vector3 grappleVector = player.position - grapplePoint;

        CmdDragBall(target, grappleVector);
    }

    [Command]
    void CmdDragBall(GameObject target, Vector3 vector)
    {
        Rigidbody ball = target.GetComponent<Rigidbody>();

        ball.AddForce(vector * grappleSpeed * 10 * Time.deltaTime);
    }
}
