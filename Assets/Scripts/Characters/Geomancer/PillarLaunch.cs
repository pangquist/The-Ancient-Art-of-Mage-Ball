using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarLaunch : Ability
{
    //Made by Kasper Veileborg
    //This script enables movement for a pillar. It checks the raycast point and spawns a pillar at the point. It has a force in y movement and stops after a while.
    //It affects the ball and can cause the ball to move if hit.
    [SerializeField] GameObject pillarPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] float range;
    [SerializeField] LayerMask[] hitableLayers;
    [SerializeField] UseAbilities useAbilities;
    [SerializeField] float cooldown;
    [SerializeField] Sprite abilityIcon;

    Transform pillarTop;
    RaycastHit hit;
    
    public override void OnStartAuthority()
    {
        enabled = true;
    }

    [Client]
    public override void UseAbility(int abilityIndex)
    {
        if (abilityIndex != 2)
        {
            return;
        }

        Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        foreach (LayerMask hitableLayer in hitableLayers)
        {
            
            Physics.Raycast(ray, out hit, range, hitableLayer);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Ground"))
            {
                CmdDoPillar(hit.point);
                useAbilities.SetOnCooldown(1, cooldown);
                break;
            }
        }

        if (hit.collider == null)
        {
            return;
        }
        
        
    }

    [Command]
    void CmdDoPillar(Vector3 hitLocation)
    {
        pillarTop = pillarPrefab.transform.GetChild(0);
        GameObject projectileInstance = Instantiate(pillarPrefab, hitLocation - pillarTop.position, Quaternion.identity);
        NetworkServer.Spawn(projectileInstance, connectionToClient);
    }

    public override Sprite ReturnIcon()
    {
        return abilityIcon;
    }
}
