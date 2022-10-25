using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private Transform vfxSmoke;
    [SerializeField] private float speed = 100;
    [SerializeField] private float lifeTime = 20.0f;
    private bool isMoving;
    private float timeLastSmoke;
    private float pointsWorth = 1;

    public float PointsWorth { get => pointsWorth; set => pointsWorth = value; }
    public bool IsMoving { get => isMoving; set => isMoving = value; }


    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            pointsWorth += (Time.deltaTime) * 10;
            lifeTime -= Time.deltaTime;
            if (lifeTime < 0f)
                Destroy(gameObject);

            if (Time.time >= timeLastSmoke + 0.04f)
            {
                timeLastSmoke = Time.time;
                Transform newFX = Instantiate(vfxSmoke, transform.position, Quaternion.identity);
                newFX.parent = GameObject.Find("/PlaneFX").transform;
            }

            transform.position += transform.forward * (speed * Time.deltaTime);
            transform.Rotate(0, 0, Time.deltaTime * 500);

        }
    }
}
