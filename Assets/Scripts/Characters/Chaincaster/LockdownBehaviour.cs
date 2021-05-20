using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockdownBehaviour : NetworkBehaviour
{
    private LineRenderer chain;
    private Material material;
    private Vector2 offsetSpeed = new Vector2(15, 0);

    [SerializeField] GameObject target;
    [SerializeField] float duration;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    void Start()
    {
        Debug.Log("Chain created");
        target = GameObject.FindGameObjectWithTag("Enemy");
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
        _target.gameObject.GetComponent<Rigidbody>().AddForce(_vector * 20 * Time.deltaTime);
    }

    void StopLockdown()
    {
        Destroy(gameObject, duration);
    }

    void DrawChain()
    {
        chain = gameObject.GetComponent<LineRenderer>();
        chain.positionCount = 2;

        chain.SetPosition(0, gameObject.transform.position);
        chain.SetPosition(1, target.transform.position);
    }
}
