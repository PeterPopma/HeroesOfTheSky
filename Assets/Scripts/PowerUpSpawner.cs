using AirplaneGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    List<string> objectNames = new List<string>()
        {
            "pfPowerupBomb",
            "pfPowerupFuel",
            "pfPowerupMissile"
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
    }

    public void SpawnPowerUps()
    {
        // Delete any existing powerups in scene
        var myPowerups = FindObjectsOfType(typeof(Powerup)); 
        foreach (Powerup powerup in myPowerups)
        {
            powerup.Destroy();
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
        GameObject newPowerUp = PhotonNetwork.Instantiate(objectName, new Vector3(spawnPosition.x,
                                                Yoffset + Random.value * (maxHeight - minHeight) + minHeight,
                                                spawnPosition.z), Quaternion.identity);
    }

}
