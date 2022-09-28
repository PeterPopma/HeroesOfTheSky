using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Drop this script on objects you want to shatter.
// Perform shatter by calling ShatterObject()
// objects must have a collider
// Read/Write must be allowed on meshes.
public class ShatterGameObjects : MonoBehaviour
{
    [Tooltip("Delay before the meshes are split (usually 0)")]
    [SerializeField] private float shatterDelay = 0f;
    [Tooltip("Delay before the exploded parts are removed")]
    [SerializeField] private float cleanupDelay = 6f;
    [Tooltip("Depth of shattering. More means more pieces.")] 
    [SerializeField] private int depth = 3;
    [Tooltip("Material used on broken faces of meshes. Default is the object's own material")] 
    [SerializeField] private Material crossSectionMaterial;
    [Tooltip("Smoke effect")]
    [SerializeField] private Transform pfSmoke;
    [Tooltip("How much the object will scatter")]
    [SerializeField] private float explosionForce = 15f;

    public void ShatterObject()
    {
        ShatterObject(transform);
    }

    public void ShatterObject(Transform root)
    {
        if (root.childCount == 0)
        {
            if (pfSmoke != null)
            {
                Instantiate(pfSmoke, transform.position, Quaternion.LookRotation(transform.position, Vector3.up));
            }
            BoxCollider boxCollider = root.gameObject.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                root.gameObject.AddComponent(typeof(BoxCollider));
            }
            Rigidbody rigidbody = root.gameObject.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                root.gameObject.AddComponent(typeof(Rigidbody));
                rigidbody = root.gameObject.GetComponent<Rigidbody>();
            }
            if (explosionForce > 0)
            {
                rigidbody.AddForce(new Vector3(Random.value, Random.value, Random.value) * explosionForce, ForceMode.VelocityChange);
                rigidbody.AddRelativeTorque(new Vector3(Random.Range(-500f, 500f), Random.Range(-500f, 500f), Random.Range(-500f, 500f)), ForceMode.VelocityChange);
            }
            //rigidbody.AddForce(Vector3.up, ForceMode.VelocityChange);
            root.parent = transform.parent;
            root.gameObject.AddComponent(typeof(ShatterMesh));
            root.gameObject.GetComponent<ShatterMesh>().CrossSectionMaterial = crossSectionMaterial;
            root.gameObject.GetComponent<ShatterMesh>().ShatterDelay = shatterDelay;
            root.gameObject.GetComponent<ShatterMesh>().CleanupDelay = cleanupDelay;
            root.gameObject.GetComponent<ShatterMesh>().Depth = depth; 
            root.gameObject.GetComponent<ShatterMesh>().PfSmoke = pfSmoke;
        }
        else
        {
            List<Transform> children = new List<Transform>();
            // first copy the children to a separate list, because they will be modified in the foreach loop.
            foreach (Transform transform in root)
            {
                children.Add(transform);
            }
            foreach (Transform transform in children)
            {
                ShatterObject(transform);
            }
            Destroy(root.gameObject);
        }
    }
}
