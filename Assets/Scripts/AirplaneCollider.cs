using Player;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AirplaneCollider : MonoBehaviour
{
    private Airplane scriptAirplane;

    public void Start()
    {
        scriptAirplane = transform.parent.parent.parent.GetComponent<Airplane>();
    }

    void OnTriggerEnter(Collider other)
    {
        scriptAirplane.Crash();
    }
}