using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockdownBehaviour : NetworkBehaviour
{
    private LineRenderer chain;
    private Material material;
    private Vector2 offsetSpeed = new Vector2(15, 0);

    [SerializeField] int count1, count2;

    [SerializeField] GameObject target;
    [SerializeField] float duration;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    void Start()
    {
        count1 = 0;
        count2 = 0;
        Debug.Log("Chain created");
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

        count1++;

        Debug.Log("Lock " + count1);

        Debug.Log("Chain update");

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
        count2++;

        Debug.Log("Lock Cmd" + count2);

        _target.gameObject.GetComponent<Rigidbody>().AddForce(_vector * 20 * Time.deltaTime);
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
        CmdDrawChain(target, gameObject, offsetSpeed);
    }

    [Command]
    void CmdDrawChain(GameObject _target, GameObject _object, Vector2 vector)
    {
        RpcDrawChain(_target, _object, vector);
    }

    [ClientRpc]
    void RpcDrawChain(GameObject _target, GameObject _object, Vector2 vector)
    {
        chain = _object.GetComponent<LineRenderer>();
        chain.positionCount = 2;

        material = chain.material;

        material.mainTextureOffset += vector * Time.deltaTime;

        chain.SetPosition(0, _object.transform.position);
        chain.SetPosition(1, _target.transform.position);
    }
}
