using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] List<string> objectNames = new List<string>()
        {
            "pfIconBomb",
            "pfIconFuel",
            "pfIconMissile"
        };
    [SerializeField] Transform rootTransform;
    [SerializeField] int initialItems = 0;
    [SerializeField] float minX = -1500;
    [SerializeField] float maxX = 1500;
    [SerializeField] float minZ = -1500;
    [SerializeField] float maxZ = 1500;
    [SerializeField] float minHeight = 100;
    [SerializeField] float maxHeight = 200;

    public void Start()
    {
        objectNames.Add("pfIconBomb");
    }

    public void SpawnPowerUps()
    {
        // Delete any existing powerups
        GameObject[] existingObjects = GameObject.FindGameObjectsWithTag("powerup");
        foreach (GameObject existingObject in existingObjects)
        {
            PhotonNetwork.Destroy(existingObject);
        }

        for (int i = 0; i < initialItems; i++)
        {
            SpawnNewObject();
        }
    }

    private void SpawnNewObject()
    {
        string objectName = objectNames[Random.Range(0, objectNames.Count)];

        Vector3 spawnPosition = new Vector3(Random.value * (maxX - minX) + minX, 2000, Random.value * (maxZ - minZ) + minZ);
        float Yoffset = Terrain.activeTerrain.SampleHeight(spawnPosition);
        PhotonNetwork.Instantiate(objectName, new Vector3(spawnPosition.x,
                                                Yoffset + Random.value * (maxHeight - minHeight) + minHeight,
                                                spawnPosition.z), Quaternion.identity);

    }

}
