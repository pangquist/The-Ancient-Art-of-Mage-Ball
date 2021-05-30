using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySwap : NetworkBehaviour
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;

    [Header("Settings")]
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask hitableLayer;
    [SerializeField] AudioSource soundEffect;

    [Header("Values")]
    [SerializeField] float range;
    [SerializeField] Vector3 appliedForce;
    [SerializeField] float duration;
    
    RaycastHit hit;
    GameObject ball;

    [Client]
    public void DoGravityWarpSpell()
    {
        if (!hasAuthority)
        {
            return;
        }

        Debug.Log("Casting Gravity Warp");

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, range, hitableLayer);

        if (hit.collider == null || hit.collider.gameObject.tag != "Enemy")
        {
            useAbilities.SetCooldownToPercentage(1, 50);
            return;
        }

        useAbilities.SetOnCooldown(1);
        Debug.Log($"Hit object name: {hit.collider.gameObject}");
        ball = hit.collider.gameObject;
        CmdDoSpell(ball);
    }

    [Command]
    void CmdDoSpell(GameObject ballObject)
    {
        if (ballObject == null)
        {
            return;
        }

        ballObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ballObject.GetComponent<Rigidbody>().AddForce(appliedForce);

        ballObject.GetComponent<BallMovement>().RevertGravity(duration);

        RpcPlaySoundEffect();
    }

    [ClientRpc]
    void RpcPlaySoundEffect()
    {
        soundEffect.Play();
    }
}
