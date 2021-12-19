using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carriage : MonoBehaviour
{

    private bool drivingRight = false;
    private Train _train;
    private void Start()
    {
        _train = GetComponentInParent<Train>();
        drivingRight = _train.driveRight;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case 9 when !drivingRight:
                _train.numberOfCarriages--;
                StartCoroutine(DestoryCarriage());
                return;
            case 10 when drivingRight:
                _train.numberOfCarriages--;
                StartCoroutine(DestoryCarriage());
                return;
        }
    }

    IEnumerator DestoryCarriage()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
