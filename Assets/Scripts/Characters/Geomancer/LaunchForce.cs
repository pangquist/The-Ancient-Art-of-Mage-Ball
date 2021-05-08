using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchForce : NetworkBehaviour
{
    [SerializeField]
    float ForceUpwards;

    Transform topPartPillar;

    bool hasPlayedAnime = false;
    bool hasCollided = false;

    [SerializeField]
    GameObject hitEffect;

    Rigidbody pushedBody;

    Vector3 originposition;
    float heightOfPillar;
    [SerializeField]
    float ballForce = 100f;

    public override void OnStartAuthority()
    {
        enabled = true;
    }
    private void Start()
    {

        topPartPillar = gameObject.transform.GetChild(0);

        originposition = gameObject.transform.position;
        heightOfPillar = (topPartPillar.position.y - originposition.y);
    }   

    private void Update()
    {
        if (!hasPlayedAnime)
        {
            CmdSpawnHitEffect();
        }
        if (gameObject.transform.position.y < originposition.y + heightOfPillar)
        {
            MovePillar();
        }
        Collider[] colliders = Physics.OverlapSphere(topPartPillar.transform.position, 5f);
        foreach (Collider pushedObject in colliders)
        {
            if (pushedObject.CompareTag("Enemy"))
            {
                
                if (!hasCollided)
                {
                    CmdPushBall(pushedObject.gameObject);
                }
                
            }
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }
    [Command]
    void CmdPushBall(GameObject ball)
    {
        ball.GetComponent<Rigidbody>().transform.Translate(0, Time.deltaTime * ForceUpwards, 0, Space.World);

            ball.GetComponent<Rigidbody>().AddForce(new Vector3(0,1,0) * ballForce, ForceMode.Force);
            hasCollided = true;
    }
    [Command]
    void CmdSpawnHitEffect()
    {
        Vector3 left = new Vector3(-2, 0, 0);
        Vector3 right = new Vector3(2, 0, 0);
        Vector3 above = new Vector3(0, 0, -2);
        Vector3 under = new Vector3(0, 0, 2);
        GameObject magicExplosionLeft = Instantiate(hitEffect, topPartPillar.transform.position + left, Quaternion.identity) as GameObject;
        GameObject magicExplosionRight = Instantiate(hitEffect, topPartPillar.transform.position + right, Quaternion.identity) as GameObject;
        GameObject magicExplosionAbove = Instantiate(hitEffect, topPartPillar.transform.position + above, Quaternion.identity) as GameObject;
        GameObject magicExplosionBelow = Instantiate(hitEffect, topPartPillar.transform.position + under, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(magicExplosionLeft);
        NetworkServer.Spawn(magicExplosionRight);
        NetworkServer.Spawn(magicExplosionAbove);
        NetworkServer.Spawn(magicExplosionBelow);
        Debug.Log($"Intantiating the hit effect:");
        hasPlayedAnime = true;
    }

    [Client]
    void MovePillar()
    {
        transform.Translate(0, Time.deltaTime * ForceUpwards, 0, Space.World);
    }
}
