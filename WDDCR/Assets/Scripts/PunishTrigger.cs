using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunishTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        other.GetComponent<DrunkAgent>()?.StayingInStartingGround();
    }
}
