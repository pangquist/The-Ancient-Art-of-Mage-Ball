using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmplifyBeamBehaviour : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Vector3 force;
    [SerializeField] Amplify amplify;
    float lifetime;
    float currentTime;

    private void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= lifetime)
        {
            currentTime = 0;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger Happened!");
        GameObject ball = other.gameObject;

        if (ball.tag == "Enemy")
        {
            amplify.MoveBall(ball, mainCamera.transform.forward * 5);
        }
    }

    public void SetLifeTime(float time)
    {
        lifetime = time;
    }

    public void SetForce(Vector3 _force)
    {
        force = _force;
    }
}
