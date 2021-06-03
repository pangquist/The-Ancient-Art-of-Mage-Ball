using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePush : Ability
{
    [Header("Script Dependencies")]
    [SerializeField] UseAbilities useAbilities;

    [Header("Settings")]
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask[] hitableLayers;
    [SerializeField] GameObject hitEffect;
    
    [Header("Values")]
    [SerializeField] float range;
    [SerializeField] float pushAmount;
    [SerializeField] float pushRadius;

    Rigidbody pushedBody;
    RaycastHit hit;

    #region Client

    private void Start()
    {
    }

    private void OnEnable()
    {
        useAbilities = GetComponent<UseAbilities>();
        hitableLayers = new LayerMask[2];
        hitableLayers[0] = LayerMask.GetMask("Jumpable");
        hitableLayers[1] = LayerMask.GetMask("Hitable");
        hitEffect = GameObject.Find("Objects").transform.Find("Hit").gameObject;

        range = 20;
        pushAmount = 700;
        pushRadius = 8;
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [Client]
    public override void UseAbility(int abilityIndex)
    {
        if (abilityIndex != 1)
        {
            return;
        }

        if (!hasAuthority)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        foreach (LayerMask hitableLayer in hitableLayers)
        {
            Physics.Raycast(ray, out hit, range, hitableLayer);
           
            if (hit.collider != null)
            {
                break;
            }
        }


        if (hit.collider == null)
        {
            Vector3 point = ray.origin + (ray.direction * range);
            CmdSpawnHitEffect(point);

            Collider[] missedColliders = Physics.OverlapSphere(point, pushRadius);
            foreach (Collider pushedObject in missedColliders)
            {
                if (pushedObject.CompareTag("Enemy"))
                {
                    Debug.Log("Client is pushing!");
                    CmdMoveBall(pushedObject.gameObject, hit.point);
                }
            }
            useAbilities.ReduceAllCooldowns(1, 0);
            useAbilities.SetOnCooldown(0);
            return;
        }

        useAbilities.ReduceAllCooldowns(1, 0);

        CmdSpawnHitEffect(hit.point);
        Collider[] colliders = Physics.OverlapSphere(hit.point, pushRadius);
        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.CompareTag("Enemy"))
            {
                Debug.Log("Client is pushing!");
                CmdMoveBall(pushedObject.gameObject, hit.point);
            }
        }

        useAbilities.SetOnCooldown(0);
    }

    #endregion
    #region Server
    
    [Command]
    void CmdMoveBall(GameObject pushedObject, Vector3 location)
    {
        pushedObject.GetComponent<Rigidbody>().AddExplosionForce(pushAmount, location, pushRadius);
    }

    [Command]
    void CmdSpawnHitEffect(Vector3 hitLocation)
    {
        GameObject magicExplosion = Instantiate(hitEffect, hitLocation, Quaternion.identity) as GameObject;
        magicExplosion.SetActive(true);
        NetworkServer.Spawn(magicExplosion);
        Debug.Log($"Instantiating the hit effect: {magicExplosion}");
    }
    #endregion
}
