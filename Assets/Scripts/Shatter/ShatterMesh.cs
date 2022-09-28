using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class ShatterMesh : MonoBehaviour {

    private float explosionForce = 15f;
    private float shatterDelay = 0f;
    private float cleanupDelay = 6f;
    private int depth = 3;
    private Material crossSectionMaterial;
    private Transform pfSmoke;

    public Material CrossSectionMaterial { get => crossSectionMaterial; set => crossSectionMaterial = value; }
    public int Depth { get => depth; set => depth = value; }
    public float ShatterDelay { get => shatterDelay; set => shatterDelay = value; }
    public float CleanupDelay { get => cleanupDelay; set => cleanupDelay = value; }
    public Transform PfSmoke { get => pfSmoke; set => pfSmoke = value; }

    public GameObject[] Shatter(GameObject objectToShatter) {
        if (crossSectionMaterial == null)
        {
            Renderer renderer = objectToShatter.GetComponent<Renderer>();
            if (renderer == null)
            {
                // not a renderable object, so return
                return null;
            }
            crossSectionMaterial = renderer.material;
        }

        /* 
        use these bounds if you want to cut at a random position within the mesh.
        for now we use Vector3.zero which is usually the center.

        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = filter.sharedMesh;
        Bounds bounds = mesh.bounds;
        */
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        Vector3 centerOfMesh = Vector3.zero;
        EzySlice.Plane cuttingPlane = new EzySlice.Plane(centerOfMesh, randomDirection);

        //cuttingPlane.Compute(transform);
        return objectToShatter.SliceInstantiate(cuttingPlane,
                                                            new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f),
                                                            crossSectionMaterial);
    }

    public EzySlice.Plane GetRandomPlane() {
        Vector3 randomPosition = Random.insideUnitSphere;
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        return new EzySlice.Plane(randomPosition, randomDirection);
    }

    public void ShatterThisObject()
    {
        PerformShatter(gameObject, depth);

        // finally delete the main object
        Destroy(gameObject);
    }

    public void Update()
    {
        shatterDelay -= Time.deltaTime;
        if (shatterDelay < 0f)
        {
            ShatterThisObject();
        }
    }

    private void PerformShatter(GameObject objectToShatter, int depthLeft) {
        depthLeft--;

        GameObject[] shatters = Shatter(objectToShatter);

        if (shatters != null && shatters.Length > 0) {
            // add rigidbodies and colliders
            foreach (GameObject shatteredObject in shatters) {
                shatteredObject.name += " depth: " + depthLeft;
                shatteredObject.AddComponent<MeshCollider>().convex = true;
                shatteredObject.AddComponent(typeof(DeleteAfterDelay));
                shatteredObject.GetComponent<DeleteAfterDelay>().delay = cleanupDelay;
                Rigidbody rigidbody = shatteredObject.AddComponent<Rigidbody>();
                if (explosionForce > 0)
                {
                    rigidbody.AddForce(new Vector3(Random.value, Random.value, Random.value) * explosionForce, ForceMode.VelocityChange);
                    rigidbody.AddTorque(new Vector3(Random.Range(-500f, 500f), Random.Range(-500f, 500f), Random.Range(-500f, 500f)), ForceMode.VelocityChange);
                }
                if (pfSmoke != null)
                {
                    Instantiate(pfSmoke, transform.position, Quaternion.LookRotation(transform.position, Vector3.up));
                }
                if (depthLeft > 0)
                {
                    PerformShatter(shatteredObject, depthLeft);
                    Destroy(shatteredObject);
                }
            }
        }
    }
}
