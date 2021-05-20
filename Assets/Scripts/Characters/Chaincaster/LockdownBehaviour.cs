using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockdownBehaviour : NetworkBehaviour
{
    private LineRenderer chain;
    private Material material;
    private Vector2 offsetSpeed = new Vector2(15, 0);
    GameObject target;

    [SerializeField] float duration;

    void Start()
    {
        Debug.Log("Chain created");
        chain = gameObject.GetComponent<LineRenderer>();
        material = chain.material;
        StopLockdown();
    }

    void Update()
    {
        Debug.Log("Chain update");

        MoveTarget(target);

        material.mainTextureOffset += offsetSpeed * Time.deltaTime;
    }

    void LateUpdate()
    {
        DrawChain(target);
    }

    public void SetTarget(GameObject _target)
    {
        target = _target;
    }

    [Client]
    void MoveTarget(GameObject _target)
    {
        if (!hasAuthority)
        {
            return;
        }

        Rigidbody ball = _target.gameObject.GetComponent<Rigidbody>();

        gameObject.transform.LookAt(_target.transform);

        Vector3 vector = gameObject.transform.position - _target.transform.position;

        ball.AddForce(vector * 20 * Time.deltaTime);
    }

    void StopLockdown()
    {
        Destroy(gameObject, duration);
    }

    //[Command]
    //void CmdStopLockdown()
    //{
    //    RpcStopLockdown();
    //}

    //[ClientRpc]
    //void RpcStopLockdown()
    //{
    //    Destroy(this);
    //}

    void DrawChain(GameObject _target)
    {
        //if (!hasAuthority || !lockActive)
        //{
        //    return;
        //}

        chain = gameObject.GetComponent<LineRenderer>();
        chain.positionCount = 2;

        chain.SetPosition(0, gameObject.transform.position);
        chain.SetPosition(1, _target.transform.position);
    }
}
