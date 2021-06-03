using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarLaunch : Ability
{
    [SerializeField] GameObject pillarPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] float range;
    [SerializeField] LayerMask[] hitableLayers;
    [SerializeField] UseAbilities useAbilities;

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
                useAbilities.SetOnCooldown(1);
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
}
