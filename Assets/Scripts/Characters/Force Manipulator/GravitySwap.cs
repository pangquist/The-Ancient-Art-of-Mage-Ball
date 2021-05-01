using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySwap : NetworkBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask hitableLayer;
    [SerializeField] GameObject attackEffect;
    [SerializeField] float range;
    [SerializeField] Vector3 appliedForce;

    [SerializeField] float duration;
    

    RaycastHit hit;

    Rigidbody affectedBody;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [Client]
    public void DoSpell()
    {
        if (!hasAuthority)
        {
            return;
        }

        Debug.Log("Casting Gravity Warp");

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, range, hitableLayer);

        if (hit.collider == null)
        {
            return;
        }

        Debug.Log($"Hit object name: {hit.collider.gameObject}");
        CmdDoSpell();
    }

    [Command]
    void CmdDoSpell()
    {
        ServerDoSpell();
    }

    [Server]
    void ServerDoSpell()
    {
        affectedBody = hit.collider.gameObject.GetComponent<Rigidbody>();

        affectedBody.velocity = Vector3.zero;
        affectedBody.AddForce(appliedForce);

        hit.collider.gameObject.GetComponent<BallMovement>().RevertGravity(duration);
    }
}
