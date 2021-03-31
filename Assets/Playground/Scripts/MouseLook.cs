using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    public float mouseSensitivity = 100f;
    [SerializeField]
    Camera playerCamera;

    float xRotation = 0f;
    float mouseX;
    float mouseY;

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
            return;

        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        gameObject.transform.Rotate(Vector3.up * mouseX);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
