using AirplaneGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private Transform vfxExplosion;
    private AudioSource soundBombExplosion;
    private Player scriptPlayer;
    private Transform cameraBomb;
    private Transform bomb;
    private bool active;
    private Vector3 speed;
    private float detonationTime;
    private bool detonated;

    private void Awake()
    {
        soundBombExplosion = GameObject.Find("/Sound/BombExplosion").GetComponent<AudioSource>();
        cameraBomb = transform.Find("Camera");
        bomb = transform.Find("Bomb");
    }

    public void SetPlayerScript(Player player)
    {
        scriptPlayer = player;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
        cameraBomb.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (!detonated)
            {
                bomb.Rotate(0,Time.deltaTime * 500,0);
                transform.Translate(speed * Time.deltaTime);
                float Yoffset = Terrain.activeTerrain.SampleHeight(transform.position);
                if (transform.position.y<Yoffset+10)
                {
                    Detonate();
                }
            }
            else
            {
                if (Time.time - detonationTime > 3)
                {
                    Destroy(gameObject);
                }
            }
        }

    }

    public void Activate(Player scriptPlayer)
    {
        transform.SetParent(null);
        GetComponent<Rigidbody>().isKinematic = false;
//        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<BoxCollider>().enabled = true;
        // place camera above bomb, looking down
        transform.rotation = Quaternion.Euler(0, scriptPlayer.transform.rotation.y, 0);
        cameraBomb.gameObject.SetActive(true);
        speed = scriptPlayer.Speed; 
        speed.y = -50;
        active = true;
    }

    private void Detonate()
    {
        if (detonated)
        {
            return;
        }
        detonated = true;
        bomb.gameObject.SetActive(false);
        scriptPlayer.CreateBomb();
        CameraShake.Instance.ShakeCamera(60, 72f);
        scriptPlayer.SoundBombDrop.Stop();
        soundBombExplosion.Play();
        detonationTime = Time.time;
        Transform newFX = Instantiate(vfxExplosion, transform.position, Quaternion.identity);
        newFX.parent = GameObject.Find("/vFX").transform;
        scriptPlayer.BombDropping = false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 40.0f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<House>() != null)
            {
                if (collider.gameObject.name.Equals("pfHouseRed"))
                {
                    GlobalParams.HealthRed -= 5;
                }
                else
                {
                    GlobalParams.HealthBlue -= 5;
                }
                Destroy(collider.gameObject);
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        Detonate();
    }
}
