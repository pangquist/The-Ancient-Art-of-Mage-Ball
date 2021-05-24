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

    [SerializeField] UseAbilities useAbilities;
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

    [Client]
    private void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

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
        if (lr.positionCount == 0) return;

        DrawChain();
    }

    [Client]
    void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(camera.position, camera.forward, out hit, maxRange))
        {
            useAbilities.SetOnCooldownAbility1();

            grapplePoint = hit.point;

            raycastHit = hit;

            CmdStartGrapple(gameObject);
        }
    }

    [Command]
    void CmdStartGrapple(GameObject caster)
    {
        RpcStartGrapple(caster);
    }

    [ClientRpc]
    void RpcStartGrapple(GameObject caster)
    {
        lr = caster.GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    [Client]
    void DrawChain()
    {
        CmdDrawChain(gameObject, castPoint.position, grapplePoint, offsetSpeed, Time.deltaTime);
    }

    [Command]
    void CmdDrawChain(GameObject caster, Vector3 _cast, Vector3 _target, Vector2 vector, float time)
    {
        RpcDrawChain(caster, _cast, _target, vector, time);
    }

    [ClientRpc]
    void RpcDrawChain(GameObject caster, Vector3 _cast, Vector3 _target, Vector2 vector, float time)
    {
        lr = caster.GetComponent<LineRenderer>();

        material = lr.material;

        material.mainTextureOffset += vector * time;

        lr.SetPosition(0, _cast);
        lr.SetPosition(1, _target);
    }

    [Client]
    void StopGrapple()
    {
        currentDuration = duration;
        CmdStopGrapple(gameObject);
    }

    [Command]
    void CmdStopGrapple(GameObject caster)
    {
        RpcStopGrapple(caster);
    }

    [ClientRpc]
    void RpcStopGrapple(GameObject caster)
    {
        lr = caster.GetComponent<LineRenderer>();
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

        CmdDragBall(target, grappleVector, Time.deltaTime);
    }

    [Command]
    void CmdDragBall(GameObject target, Vector3 vector, float time)
    {
        Rigidbody ball = target.GetComponent<Rigidbody>();

        ball.AddForce(vector * grappleSpeed * 10 * time);
    }
}
