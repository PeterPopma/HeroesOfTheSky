using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconBomb : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    float offset;

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
        PhotonNetwork.Destroy(gameObject);
    }
}
