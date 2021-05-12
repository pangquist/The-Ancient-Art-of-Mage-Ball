using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockdown : NetworkBehaviour
{
    private LineRenderer chain1, chain2, chain3;
    private GameObject anchor1, anchor2, anchor3;
    private RaycastHit raycastHit;
    private GameObject target;
    private PlayerMovement playerMovement;
    private Vector2 offsetSpeed = new Vector2(15, 0);

    [SerializeField] GameObject chain;
    [SerializeField] Material material;
    [SerializeField] LayerMask grappleable;
    [SerializeField] Transform castPoint, camera, player;
    [SerializeField] private float maxRange = 25f;
    [SerializeField] float duration;
    [SerializeField] float currentDuration;

    [SerializeField] bool lockActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        currentDuration = duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (!lockActive)
        {
            return;
        }

        currentDuration -= Time.deltaTime;

        material.mainTextureOffset += offsetSpeed * Time.deltaTime;

        if (currentDuration <= 0)
        {
            StopLockdown();
        }

        Rigidbody ball = target.gameObject.GetComponent<Rigidbody>();

        anchor1.transform.LookAt(target.transform);
        anchor2.transform.LookAt(target.transform);
        anchor3.transform.LookAt(target.transform);

        Vector3 vector1 = anchor1.transform.position - target.transform.position;
        Vector3 vector2 = anchor2.transform.position - target.transform.position;
        Vector3 vector3 = anchor2.transform.position - target.transform.position;

        ball.AddForce(vector1 * 20 * Time.deltaTime);
        ball.AddForce(vector2 * 20 * Time.deltaTime);
        ball.AddForce(vector3 * 20 * Time.deltaTime);
    }

    private void LateUpdate()
    {
        DrawChains();
    }


    void CastLockdown()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxRange))
        {
            target = hit.transform.gameObject;

            anchor1 = Instantiate(chain, target.transform.position + Random.insideUnitSphere * 5, transform.rotation) as GameObject;
            anchor2 = Instantiate(chain, target.transform.position + Random.insideUnitSphere * 5, transform.rotation) as GameObject;
            anchor3 = Instantiate(chain, target.transform.position + Random.insideUnitSphere * 5, transform.rotation) as GameObject;

            NetworkServer.Spawn(anchor1);
            NetworkServer.Spawn(anchor2);
            NetworkServer.Spawn(anchor3);

            chain1 = anchor1.GetComponent<LineRenderer>();
            chain2 = anchor2.GetComponent<LineRenderer>();
            chain3 = anchor3.GetComponent<LineRenderer>();
            chain1.positionCount = 2;
            chain2.positionCount = 2;
            chain3.positionCount = 2;

            lockActive = true;

            raycastHit = hit;
        }
    }

    void StopLockdown()
    {
        Destroy(anchor1);
        Destroy(anchor2);
        Destroy(anchor3);

        currentDuration = duration;

        lockActive = false;
    }

    void DrawChains()
    {
        if (!lockActive) return;

        chain1.SetPosition(0, anchor1.transform.position);
        chain1.SetPosition(1, target.transform.position);

        chain2.SetPosition(0, anchor2.transform.position);
        chain2.SetPosition(1, target.transform.position);

        chain3.SetPosition(0, anchor3.transform.position );
        chain3.SetPosition(1, target.transform.position);
    }
}
