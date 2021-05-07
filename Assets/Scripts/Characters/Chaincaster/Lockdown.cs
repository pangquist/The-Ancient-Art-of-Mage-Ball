using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockdown : NetworkBehaviour
{
    private LineRenderer lr, targetLr1, targetLr2, targetLr3;
    private GameObject targetPoint1, targetPoint2, targetPoint3;
    private Material material;
    private RaycastHit raycastHit;
    private GameObject target;
    private PlayerMovement playerMovement;

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
        lr = GetComponent<LineRenderer>();

        lr.positionCount = 0;

        material = lr.material;

        targetPoint1 = new GameObject();
        targetPoint2 = new GameObject();
        targetPoint3 = new GameObject();

        targetLr1 = targetPoint1.AddComponent<LineRenderer>();
        targetLr2 = targetPoint2.AddComponent<LineRenderer>();
        targetLr3 = targetPoint3.AddComponent<LineRenderer>();

        targetLr1.positionCount = 0;
        targetLr1.material = lr.material;

        targetLr2.positionCount = 0;
        targetLr2.material = lr.material;

        targetLr3.positionCount = 0;
        targetLr3.material = lr.material;

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

        if (currentDuration <= 0)
        {
            StopLockdown();
        }

        Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, 5f);
        foreach (var hitCollider in hitColliders)
        {
            if(targetPoint1.transform.position == Vector3.zero)
            {
                targetPoint1.transform.position = hitCollider.ClosestPoint(target.transform.position);
            }
            else if (targetPoint2.transform.position == Vector3.zero)
            {
                targetPoint2.transform.position = hitCollider.ClosestPoint(target.transform.position);
            }
            else if (targetPoint3.transform.position == Vector3.zero)
            {
                targetPoint3.transform.position = hitCollider.ClosestPoint(target.transform.position);
            }
        }
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

            targetLr1.positionCount = 2;
            targetLr2.positionCount = 2;
            targetLr3.positionCount = 2;

            lockActive = true;

            raycastHit = hit;
        }
    }

    void StopLockdown()
    {
        currentDuration = duration;
        targetLr1.positionCount = 0;
        targetLr2.positionCount = 0;
        targetLr3.positionCount = 0;

        targetPoint1.transform.position = Vector3.zero;
        targetPoint2.transform.position = Vector3.zero;
        targetPoint3.transform.position = Vector3.zero;
    }

    void DrawChains()
    {
        if (targetLr1.positionCount == 0) return;

        targetLr1.SetPosition(0, targetPoint1.transform.position);
        targetLr1.SetPosition(1, raycastHit.point);

        targetLr2.SetPosition(0, targetPoint2.transform.position);
        targetLr2.SetPosition(1, raycastHit.point);

        targetLr3.SetPosition(0, targetPoint3.transform.position );
        targetLr3.SetPosition(1, raycastHit.point);
    }
}
