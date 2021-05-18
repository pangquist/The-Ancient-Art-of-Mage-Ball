using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PillarMovement : NetworkBehaviour
{
    [SerializeField] GameObject pillarSmoke;
    [SerializeField] Transform pillarTop;

    public GameObject Player { get => Player; set => Player = value; }

    private void Start()
    {
        CmdSpawnHitEffect(gameObject.transform.position);
    }

    [Client]
    private void Update()
    {
        gameObject.transform.position = Player.transform.position - pillarTop.transform.position;
        Vector3 effectPosition = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
        CmdMoveHitEffect(effectPosition);
    }

    [Command]
    void CmdMoveHitEffect(Vector3 effectPosition)
    {
        pillarSmoke.transform.Translate(effectPosition, Space.World);
    }

    [Command]
    void CmdSpawnHitEffect(Vector3 effectPosition)
    {
        GameObject pillarSmokeEffect = Instantiate(pillarSmoke, effectPosition, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(pillarSmoke);
    }
}
