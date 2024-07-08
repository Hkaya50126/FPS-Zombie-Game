using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MouseLook : MonoBehaviour
{
    public float mouseSensivity = 500f;
    private float mouseX;
    private float mouseY;
    public Transform characterTransform;
    float xRotation = 0f;
    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }
        mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        characterTransform.Rotate(Vector3.up * mouseX);
    }
}
