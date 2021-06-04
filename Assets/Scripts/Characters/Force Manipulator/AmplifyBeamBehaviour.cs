using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmplifyBeamBehaviour : MonoBehaviour
{
    [Header("Script Dependencies")]
    [SerializeField] Amplify amplify;

    [Header("Settings")]
    [SerializeField] Camera mainCamera;

    [Header("Values")]
    [SerializeField] float force;

    float lifetime;
    float currentTime;

    private void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        amplify = gameObject.transform.parent.gameObject.GetComponentInParent<Amplify>();
        mainCamera = gameObject.GetComponentInParent<Camera>();
        force = 20;
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
