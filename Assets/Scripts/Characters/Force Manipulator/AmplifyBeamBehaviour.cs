using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmplifyBeamBehaviour : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Amplify amplify;
    [SerializeField] float force;
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
            amplify.MoveBall(ball, mainCamera.transform.forward * force);
        }
    }

    public void SetLifeTime(float time)
    {
        lifetime = time;
    }

    public void SetForce(float _force)
    {
        force = _force;
    }
}
