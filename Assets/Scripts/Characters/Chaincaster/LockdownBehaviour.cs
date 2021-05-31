using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockdownBehaviour : NetworkBehaviour
{
    private LineRenderer chain;
    private Material material;
    private Vector2 offsetSpeed = new Vector2(15, 0);

    [Header("Values")]
    [SerializeField] GameObject target;
    [SerializeField] float force;
    [SerializeField] float duration;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Enemy");
        chain = gameObject.GetComponent<LineRenderer>();
        material = chain.material;
        StopLockdown();
    }

    [Client]
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        MoveTarget(target);
    }

    void LateUpdate()
    {
        DrawChain();
    }

    [Client]
    void MoveTarget(GameObject _target)
    {
        if (!hasAuthority)
        {
            return;
        }

        gameObject.transform.LookAt(_target.transform);

        Vector3 vector = gameObject.transform.position - _target.transform.position;

        CmdMoveTarget(_target, vector);
    }

    [Command]
    void CmdMoveTarget(GameObject _target, Vector3 _vector)
    {
        _target.gameObject.GetComponent<Rigidbody>().AddForce(_vector * force * Time.deltaTime);
    }

    [Client]
    void StopLockdown()
    {
        CmdStopLockdown(gameObject);
    }

    [Command]
    void CmdStopLockdown(GameObject _object)
    {
        Destroy(_object, duration);
    }

    [Client]
    void DrawChain()
    {
        CmdDrawChain(target, gameObject, offsetSpeed, Time.deltaTime);
    }

    [Command]
    void CmdDrawChain(GameObject _target, GameObject _object, Vector2 vector, float time)
    {
        RpcDrawChain(_target, _object, vector, time);
    }

    [ClientRpc]
    void RpcDrawChain(GameObject _target, GameObject _object, Vector2 vector, float time)
    {
        chain = _object.GetComponent<LineRenderer>();
        chain.positionCount = 2;

        material = chain.material;

        material.mainTextureOffset += vector * time;

        chain.SetPosition(0, _object.transform.position);
        chain.SetPosition(1, _target.transform.position);
    }
}
