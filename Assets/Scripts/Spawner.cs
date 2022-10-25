using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] pfSpawnObjects;
    [SerializeField] Transform rootTransform;
    [SerializeField] float secondsBetweenSpawns = 1;
    [SerializeField] int initialItems = 0;
    [SerializeField] float minX = -1500;
    [SerializeField] float maxX = 1500;
    [SerializeField] float minZ = -1500;
    [SerializeField] float maxZ = 1500;
    [SerializeField] float minHeight = 100;
    [SerializeField] float maxHeight = 200;

    float timeLastSpawn;

    // Start is called before the first frame update
    void Start()
    {
        timeLastSpawn = Time.time;
        for (int i = 0; i < initialItems; i++)
        {
            SpawnNewObject();
        }
    }

    private void SpawnNewObject()
    {
        int objectIndex = Random.Range(0, pfSpawnObjects.Length);

        Vector3 spawnPosition = new Vector3(Random.value * (maxX - minX) + minX, 2000, Random.value * (maxZ - minZ) + minZ);
        float Yoffset = Terrain.activeTerrain.SampleHeight(spawnPosition);
        GameObject newObject = Instantiate(pfSpawnObjects[objectIndex], new Vector3(spawnPosition.x,
                                                Yoffset + Random.value * (maxHeight - minHeight) + minHeight,
                                                spawnPosition.z),
                                                Quaternion.identity); 

        newObject.transform.parent = rootTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if (secondsBetweenSpawns>0 && Time.time - timeLastSpawn > secondsBetweenSpawns)
        {
            SpawnNewObject();
            timeLastSpawn = Time.time;
        }
    }
}
