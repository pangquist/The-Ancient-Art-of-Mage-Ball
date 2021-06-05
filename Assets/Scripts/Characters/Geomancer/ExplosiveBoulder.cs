using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBoulder : Ability
{
    //Made by Kasper
    //The scripts spawns a boulder and adds a force in the direction of the players aim. The boulder has a trail attached to it and a 
    [Header("Dependency")]
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform boulderStartPoint;
    [SerializeField] LayerMask [] hitableLayers;    
    [SerializeField] GameObject boulderPrefab;
    [SerializeField] GameObject hitableObject;
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] Sprite abilityIcon;

    [Header("Values")]
    [SerializeField] float cooldown;


    RaycastHit hit;

    public float throwForceForward;
    public float throwForceUpward;  

    #region Client
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
        //Filter out layers that are not solid.
        foreach (LayerMask hitableLayer in hitableLayers)
        {
            Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, hitableLayer);
        }
        //direction of the throw
        Vector3 directionOfBoulder;
        if(hit.collider != null)
        {
            directionOfBoulder = hit.point - boulderStartPoint.position;            
        }
        else
        {
            directionOfBoulder =  mainCamera.transform.forward;
        }
        Vector3 forceDirection = mainCamera.transform.forward;

       
        CmdBoulderThrow(directionOfBoulder, boulderStartPoint.position, forceDirection, throwForceForward); //calls command to spawn and add force to the instantiated object
        useAbilities.SetOnCooldown(0, cooldown);
    }

    #endregion

    #region Server
    [Command]
    void CmdBoulderThrow(Vector3 directionOfBoulder, Vector3 startPos, Vector3 direction, float throwForceForward)
    {
        GameObject projectileInstance = Instantiate(boulderPrefab, startPos, Quaternion.identity);
        NetworkServer.Spawn(projectileInstance);
        projectileInstance.GetComponent<Rigidbody>().AddForce(directionOfBoulder.normalized * throwForceForward, ForceMode.Force);
        projectileInstance.GetComponent<Rigidbody>().AddForce(Vector3.up * throwForceUpward, ForceMode.Force);
        
    }

    public override Sprite ReturnIcon()
    {
        return abilityIcon;
    }
    #endregion
}
