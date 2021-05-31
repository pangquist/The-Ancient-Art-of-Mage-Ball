using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainGrapple : NetworkBehaviour
{
    private LineRenderer lr;
    private Material material;
    private Vector2 offsetSpeed = new Vector2(15, 0);
    private Vector3 grapplePoint;
    private RaycastHit raycastHit;
    private GameObject grappleObject;
    private GameObject activeGrapple;
    private AudioSource chainSound;

    [Header("Dependencies")]
    [SerializeField] MyNetworkPlayer playerInfo;
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] CharacterController controller;
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform castPoint;

    [Header("Values")]
    [SerializeField] private float maxRange = 25f;
    [SerializeField] float grappleSpeed;
    [SerializeField] float dragSpeed;
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

        grappleObject = new GameObject("GrappleObject");

        chainSound = gameObject.GetComponent<AudioSource>();
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

        grapplePoint = activeGrapple.transform.position;

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
            Vector3 grappleVector = grapplePoint - gameObject.transform.position;

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
        bool hitBall = false;
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxRange))
        {
            activeGrapple = Instantiate(grappleObject, hit.point, transform.rotation) as GameObject;

            activeGrapple.transform.parent = hit.transform;

            raycastHit = hit;

            if(hit.transform.gameObject.tag == "Enemy")
            {
                hitBall = true;
            }

            CmdStartGrapple(gameObject);
        }

        if (hitBall)
        {
            useAbilities.SetOnCooldown(0);
        }
        else
        {
            useAbilities.SetCooldownToPercentage(0, 50);
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
        chainSound.mute = false;
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
        Destroy(activeGrapple);
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
        chainSound.mute = true;
    }

    [Client]
    void DragBall(GameObject target)
    {
        if (!hasAuthority)
        {
            return;
        }

        Vector3 grappleVector = gameObject.transform.position - grapplePoint;

        CmdDragBall(target, grappleVector, Time.deltaTime);
    }

    [Command]
    void CmdDragBall(GameObject target, Vector3 vector, float time)
    {
        Rigidbody ball = target.GetComponent<Rigidbody>();

        ball.AddForce(vector * dragSpeed * time);
    }
}
