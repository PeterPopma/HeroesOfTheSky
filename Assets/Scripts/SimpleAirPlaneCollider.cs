using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class SimpleAirPlaneCollider : MonoBehaviour
    {
        public bool collideSometing;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<SimpleAirPlaneCollider>() == null)
            {
                collideSometing = true;
            }
        }
    }
}