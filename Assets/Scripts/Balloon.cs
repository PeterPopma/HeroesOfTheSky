using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    private Transform hitEffectSpawnPosition;
    private AudioSource soundRocketExplosion;
    private Game scriptGame;

    void Awake()
    {
        scriptGame = GameObject.Find("/Scripts/Game").GetComponent<Game>();
        hitEffectSpawnPosition = transform.Find("HitEffectSpawnPosition");
        soundRocketExplosion = GameObject.Find("/Sound/MissileHit").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Time.deltaTime * 100, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Missile missile = other.gameObject.GetComponent<Missile>();
        if (missile != null && missile.IsMoving)      // don't count the rockets on the wings
        {
//            scriptGame.IncreaseScore(Convert.ToInt32(missile.PointsWorth));
            soundRocketExplosion.Play();
            Instantiate(vfxHit, hitEffectSpawnPosition.position, vfxHit.transform.rotation);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
