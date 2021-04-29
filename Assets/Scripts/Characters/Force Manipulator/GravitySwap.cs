using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySwap : NetworkBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask hitableLayer;
    [SerializeField] GameObject attackEffect;
    [SerializeField] Vector3 appliedForce;

    float cooldown;
    float range;

    RaycastHit hit;

    Rigidbody affectedBody;

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    public void DoSpell()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, range, hitableLayer);

        if (hit.collider == null)
        {
            return;
        }

        hit.collider.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(appliedForce);
        hit.collider.gameObject.GetComponent<Rigidbody>().useGravity = false;

    }
}
