using AirplaneGame;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace AirplaneGame
{

    public class PlayerCollider : MonoBehaviour
    {
        private const float MINIMUM_DAMAGE_SPEED = 30;
        private Player scriptPlayer;

        private void Awake()
        {
        }

        public void Start()
        {
            scriptPlayer = transform.parent.parent.GetComponent<Player>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.name.StartsWith("pfPowerupMissile"))
            {
                scriptPlayer.AddMissiles();
                other.GetComponent<Powerup>().HitTarget();
            }
            else if(other.name.StartsWith("pfPowerupBomb"))
            {
                scriptPlayer.AddBombs();
                other.GetComponent<Powerup>().HitTarget();
            }
            else if (other.name.StartsWith("pfPowerupFuel"))
            {
                scriptPlayer.AddFuel();
                other.GetComponent<Powerup>().HitTarget();
            }
            else if (other.GetComponent<Missile>() != null && other.GetComponent<Missile>().OwnerIsRed == scriptPlayer.IsRedPlane)
            { 
                // Hit own plane with missile
            }
            else if(!name.Equals("Icon"))
            {
                if (scriptPlayer.CurrentSpeed > MINIMUM_DAMAGE_SPEED)
                {
                    scriptPlayer.Crash();
                }
            }
        }
    }
}