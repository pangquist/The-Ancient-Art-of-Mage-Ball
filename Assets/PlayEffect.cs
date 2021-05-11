using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffect : NetworkBehaviour
{
    [SerializeField] GameObject smokeEffect;


    [Command]
    public void Play()
    {
        smokeEffect.transform.position = new Vector3(1, 1, 1);
        smokeEffect.SetActive(true);
    }

}
