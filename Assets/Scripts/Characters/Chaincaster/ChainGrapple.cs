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
    private Vector3 grapplePoint, pointOffset;

    [SerializeField] UseAbilities useAbilities;
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask grappleable;
    [SerializeField] Transform castPoint, camera, player;
    [SerializeField] private float maxRange = 25f;
    [SerializeField] float grappleSpeed;
    [SerializeField] float dragSpeed;
    [SerializeField] float duration;
    [SerializeField] float currentDuration;

    [SerializeField] MyNetworkPlayer playerInfo;

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

        if (GameObject.Find("GamestateManager").GetComponent<GamestateManager>().matchIsPaused)
        {
            StopGrapple();
        }

        if (Input.GetMouseButton(0) == false && currentDuration <= duration / 2)
        {
            StopGrapple();
        }

        if (lr.positionCount == 0) return;

        currentDuration -= Time.deltaTime;

        grapplePoint = raycastHit.transform.position + pointOffset;

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
            Vector3 grappleVector;

            //if (raycastHit.transform.gameObject.GetComponent<Rigidbody>() != null || raycastHit.transform.gameObject.GetComponent<CharacterController>() != null)
            //{
            //    grappleVector = raycastHit.transform.position - player.position;
            //}
            //else
            //{
            //    grappleVector = grapplePoint - player.position;
            //}

            grappleVector = grapplePoint - player.position;

            controller.Move(grappleVector * grappleSpeed * Time.deltaTime);
        }
        
    }

    private void LateUpdate()
    {
        DrawChain();
    }

    [Client]
    void StartGrapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(camera.position, camera.forward, out hit, maxRange))
        {
            useAbilities.SetOnCooldownAbility1();

            pointOffset = hit.transform.gameObject.transform.position - hit.point;

            //pointOffset = Vector3.

            //grapplePoint = hit.point;

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

        if (lr.positionCount == 0) return;

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

        //grapplePoint = target.transform.position;

        Vector3 grappleVector = player.position - grapplePoint;

        CmdDragBall(target, grappleVector, Time.deltaTime);
    }

    [Command]
    void CmdDragBall(GameObject target, Vector3 vector, float time)
    {
        Rigidbody ball = target.GetComponent<Rigidbody>();

        ball.AddForce(vector * dragSpeed * time);
    }
}
