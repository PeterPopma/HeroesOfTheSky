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
            if (other.GetComponent<IconMissile>() != null)
            {
                scriptPlayer.AddMissiles();
                other.GetComponent<IconMissile>().HitTarget();
            }
            else if(other.GetComponent<IconBomb>() != null)
            {
                scriptPlayer.AddBombs();
                other.GetComponent<IconBomb>().HitTarget();
            }
            else if (other.GetComponent<IconFuel>() != null)
            {
                scriptPlayer.AddFuel();
                other.GetComponent<IconFuel>().HitTarget();
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