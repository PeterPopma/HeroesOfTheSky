using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private AudioSource soundBombExplosion;

    private void Awake()
    {
        soundBombExplosion = GameObject.Find("/Sound/BombExplosion").GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        CameraShake.Instance.ShakeCamera(60, 72f);
        soundBombExplosion.Play();
    }
}
