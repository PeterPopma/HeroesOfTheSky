using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private Transform vfxSmoke;
    [SerializeField] private float speed = 100;
    [SerializeField] private float lifeTime = 20.0f;
    private bool isMoving = false;
    private float timeLastSmoke;
    private float pointsWorth = 1;

    public float PointsWorth { get => pointsWorth; set => pointsWorth = value; }
    public bool IsMoving { get => isMoving; set => isMoving = value; }

    private void Awake()
    {
    }
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            pointsWorth += (Time.deltaTime) * 10;
            lifeTime -= Time.deltaTime;
            if (lifeTime < 0f)
                Destroy(this.gameObject);

            if (Time.time >= timeLastSmoke + 0.04f)
            {
                timeLastSmoke = Time.time;
                Instantiate(vfxSmoke, transform.position, Quaternion.identity);
            }

            transform.position += transform.forward * (speed * Time.deltaTime);
        }
    }
}