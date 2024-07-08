using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class WeaponSway : MonoBehaviour
{

    Vector3 startPos;
    public float swaySensivity = 2f;
    public float swayClamp = 20f;
    public float swaySmoothness = 20f;
    Vector3 nextPos;
    Vector3 currentVelocity = Vector3.zero;

    public PhotonView photonView;


    void Start()
    {
        startPos= transform.position;
    }

    
    void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }
        float mouseX = Input.GetAxis("Mouse X") * swaySensivity / 100 * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * swaySensivity / 100 * Time.deltaTime;

        mouseX = Mathf.Clamp(mouseX, -swayClamp, swayClamp);
        mouseY = Mathf.Clamp(mouseY, -swayClamp, swayClamp);

        nextPos = new Vector3(mouseX, mouseY, 0);

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, nextPos + startPos, ref currentVelocity, swaySmoothness * Time.deltaTime);
    }
}
