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
        
        if (other.gameObject.layer == 9 && !drivingRight)
        {
            
            _train.numberOfCarriages--;
            Destroy(gameObject);
            return;
        }
        if (other.gameObject.layer == 10 && drivingRight)
        {
            
            _train.numberOfCarriages--;
            Destroy(gameObject);
            return;
        }
    }
}
