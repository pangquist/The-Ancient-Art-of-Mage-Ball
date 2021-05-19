using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictCasting : NetworkBehaviour
{
    [SerializeField] LayerMask grappleable;
    [SerializeField] Transform castPoint, camera, player;
    [SerializeField] private float maxRange = 25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CastRestrict()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxRange))
        {
            if(hit.transform.gameObject.tag == "Player")
            {
                UseAbilities targetAbilities = hit.transform.gameObject.GetComponent<UseAbilities>();

                targetAbilities.StartAllCooldowns();
            }
        }
    }
}
