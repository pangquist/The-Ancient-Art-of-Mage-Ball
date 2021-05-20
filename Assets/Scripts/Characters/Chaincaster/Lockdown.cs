using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockdown : NetworkBehaviour
{
    private LineRenderer chain1, chain2, chain3;
    private GameObject anchor1, anchor2, anchor3;
    private GameObject target;
    private Vector2 offsetSpeed = new Vector2(15, 0);

    [SerializeField] GameObject chain;
    [SerializeField] Material material;
    [SerializeField] LayerMask grappleable;
    [SerializeField] Transform castPoint, camera, player;
    [SerializeField] private float maxRange = 25f;
    [SerializeField] float duration;
    [SerializeField] float currentDuration;

    [SerializeField] bool lockActive;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        currentDuration = duration;
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (!lockActive)
    //    {
    //        return;
    //    }

    //    currentDuration -= Time.deltaTime;

    //    material.mainTextureOffset += offsetSpeed * Time.deltaTime;

    //    if (currentDuration <= 0)
    //    {
    //        StopLockdown();
    //        return;
    //    }

    //    MoveTarget(target, anchor1, anchor2, anchor3);
    //}

    //[Client]
    //void MoveTarget(GameObject _target, GameObject _anchor1, GameObject _anchor2, GameObject _anchor3)
    //{
    //    if (!hasAuthority)
    //    {
    //        return;
    //    }

    //    Rigidbody ball = _target.gameObject.GetComponent<Rigidbody>();

    //    _anchor1.transform.LookAt(_target.transform);
    //    _anchor2.transform.LookAt(_target.transform);
    //    _anchor3.transform.LookAt(_target.transform);

    //    Vector3 vector1 = _anchor1.transform.position - _target.transform.position;
    //    Vector3 vector2 = _anchor2.transform.position - _target.transform.position;
    //    Vector3 vector3 = _anchor2.transform.position - _target.transform.position;

    //    ball.AddForce(vector1 * 20 * Time.deltaTime);
    //    ball.AddForce(vector2 * 20 * Time.deltaTime);
    //    ball.AddForce(vector3 * 20 * Time.deltaTime);
    //}

    //private void LateUpdate()
    //{
    //    DrawChains(target, anchor1, anchor2, anchor3, chain1, chain2, chain3);
    //}

    [Client]
    void CastLockdown()
    {
        if(!hasAuthority)
        {
            return;
        }

        Debug.Log("Casting Lockdown");

        //NetworkClient.Ready();

        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxRange))
        {
            target = hit.transform.gameObject;

            if(!(target.tag == "Player" || target.tag == "Enemy"))
            {
                return;
            }
           
            CmdCastSpell(target);

            lockActive = true;
        }
    }

    [Command]
    void CmdCastSpell(GameObject target)
    {
        Debug.Log("Casting Lockdown 2");
        RpcCastSpell(target);
    }

    [ClientRpc]
    void RpcCastSpell(GameObject target)
    {
        Debug.Log("Casting Lockdown 3");
        if (NetworkServer.active)
        {
            anchor1 = Instantiate(chain, target.transform.position + Random.insideUnitSphere * 5, transform.rotation) as GameObject;
            anchor2 = Instantiate(chain, target.transform.position + Random.insideUnitSphere * 5, transform.rotation) as GameObject;
            anchor3 = Instantiate(chain, target.transform.position + Random.insideUnitSphere * 5, transform.rotation) as GameObject;

            NetworkServer.Spawn(anchor1, connectionToClient);
            NetworkServer.Spawn(anchor2, connectionToClient);
            NetworkServer.Spawn(anchor3, connectionToClient);

            LockdownBehaviour behave1 = anchor1.GetComponent<LockdownBehaviour>();
            LockdownBehaviour behave2 = anchor2.GetComponent<LockdownBehaviour>();
            LockdownBehaviour behave3 = anchor3.GetComponent<LockdownBehaviour>();

            behave1.SetTarget(target);
            behave2.SetTarget(target);
            behave3.SetTarget(target);

            //chain1 = anchor1.GetComponent<LineRenderer>();
            //chain2 = anchor2.GetComponent<LineRenderer>();
            //chain3 = anchor3.GetComponent<LineRenderer>();
            //chain1.positionCount = 2;
            //chain2.positionCount = 2;
            //chain3.positionCount = 2;
        }
    }

    //[Client]
    //void StopLockdown()
    //{
    //    if (!hasAuthority)
    //    {
    //        return;
    //    }

    //    CmdStopLockdown();

    //    currentDuration = duration;

    //    lockActive = false;
    //}

    //[Command]
    //void CmdStopLockdown()
    //{
    //    RpcStopLockdown();
    //}

    //[ClientRpc]
    //void RpcStopLockdown()
    //{
    //    Destroy(anchor1);
    //    Destroy(anchor2);
    //    Destroy(anchor3);
    //}

    //[Client]
    //void DrawChains(GameObject _target, GameObject _anchor1, GameObject _anchor2, GameObject _anchor3, LineRenderer _chain1, LineRenderer _chain2, LineRenderer _chain3)
    //{
    //    if (!hasAuthority || !lockActive)
    //    {
    //        return;
    //    }

    //    chain1 = _anchor1.GetComponent<LineRenderer>();
    //    chain2 = _anchor2.GetComponent<LineRenderer>();
    //    chain3 = _anchor3.GetComponent<LineRenderer>();
    //    chain1.positionCount = 2;
    //    chain2.positionCount = 2;
    //    chain3.positionCount = 2;

    //    chain1.SetPosition(0, _anchor1.transform.position);
    //    chain1.SetPosition(1, _target.transform.position);

    //    chain2.SetPosition(0, _anchor2.transform.position);
    //    chain2.SetPosition(1, _target.transform.position);

    //    chain3.SetPosition(0, _anchor3.transform.position);
    //    chain3.SetPosition(1, _target.transform.position);
    //}
}
