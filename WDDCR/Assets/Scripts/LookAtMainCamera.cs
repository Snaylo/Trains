using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
    private Vector3 mainCamPos;


    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
