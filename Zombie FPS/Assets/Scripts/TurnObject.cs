using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnObject : MonoBehaviour
{
    public float rotateSpeed;
    void Update()
    {
        if (gameObject.tag=="BulletTag")
        {
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }
        else
        {
            transform.Rotate(40 * Time.deltaTime, rotateSpeed * Time.deltaTime, 40 * Time.deltaTime);
        }

    }
}
