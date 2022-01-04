using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    private float _speed = 1.0f;
    public bool driveRight = false;
    public int numberOfCarriages = 1;
    public int track;
    private bool _terminate;
    public void SetSpeed(float speed)
    {
        _speed = speed;
        
    }

    private void Update()
    {
        if(_terminate) return;
        if (numberOfCarriages <= 0)
        {
            _terminate = true;
            TrainManager.Instance.tracksOccupied[track] = false;
            StartCoroutine(DestroyTrain());
            return;
        }
        if (driveRight)transform.position += new Vector3(0 , 0, _speed * Time.deltaTime);
        else transform.position -= new Vector3(0 , 0, _speed * Time.deltaTime);
    }

    IEnumerator DestroyTrain()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
