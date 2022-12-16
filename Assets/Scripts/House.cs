using AirplaneGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private Transform vfxExplosion;
    private Game scriptGame;
    private float timeDestroyed;
    private bool isDestroyed = false;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        scriptGame = GameObject.Find("/Scripts/Game").GetComponent<Game>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDestroyed)
        {
            if (Time.time - timeDestroyed>5)
            {
                Destroy();
            }
        }
    }

    public void Explode()
    {
        Transform newFX = Instantiate(vfxExplosion, transform.position, Quaternion.identity);
        newFX.parent = GameObject.Find("/vFX").transform;
        isDestroyed = true;
        timeDestroyed = Time.time;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(new Vector3(Random.Range(-30f, 30f), 10, Random.Range(-30f, 30f)), ForceMode.Impulse);
        rigidbody.AddTorque(new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2)), ForceMode.VelocityChange);
        rigidbody.useGravity = true;

        if (name.StartsWith("pfHouseRed"))
        {
            scriptGame.PlayerRed.GetComponent<Player>().DecreaseHealth();
        }
        else
        {
            scriptGame.PlayerBlue.GetComponent<Player>().DecreaseHealth();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDestroyed)
        {
            return;
        }

        if (other.name.Equals("pfBomb"))
        {
            // Bomb has its own collision detection
            return;
        }

        Explode();
    }

    public void Destroy()
    {
        photonView.RPC("DestroyHouse", RpcTarget.All);
    }

    [PunRPC]
    public void DestroyHouse()
    {
        // The read player created the powerups, so should also destroy them.
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
        }
    }
}
