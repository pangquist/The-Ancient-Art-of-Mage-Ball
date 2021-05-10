using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] Camera playerCamera;

    float xRotation = 0f;
    float mouseXRotation;
    float mouseYRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnStartAuthority()
    {
        playerCamera.gameObject.GetComponent<Camera>().enabled = true;
        enabled = true;
    }
    
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        mouseXRotation = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseYRotation = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseYRotation;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        gameObject.transform.Rotate(Vector3.up * mouseXRotation);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
