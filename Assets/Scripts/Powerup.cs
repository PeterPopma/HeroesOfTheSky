using AirplaneGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    float offset;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        offset = Random.value;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, offset + Time.deltaTime * 50, 0);
    }

    public void HitTarget()
    {
        Instantiate(vfxHit, transform.position, vfxHit.transform.rotation);
        Destroy();
    }

    public void Destroy()
    {
        photonView.RPC("DestroyPowerup", RpcTarget.All);
    }


    [PunRPC]
    public void DestroyPowerup()
    {
        // The read player created the powerups, so should also destroy them.
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
