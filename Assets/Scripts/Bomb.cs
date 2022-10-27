using AirplaneGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private AudioSource soundBombExplosion;
    private Player scriptPlayer;

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

    public void Activate(Player scriptPlayer)
    {
        this.scriptPlayer = scriptPlayer;
        GetComponent<BoxCollider>().enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        CameraShake.Instance.ShakeCamera(60, 72f);
        scriptPlayer.SoundBombDrop.Stop();
        soundBombExplosion.Play();
        Destroy(gameObject);
    }
}
