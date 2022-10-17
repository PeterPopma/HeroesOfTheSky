using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LandingLight : MonoBehaviour
{
    LensFlareComponentSRP lensFlare;
    [SerializeField] float intensity;
    [SerializeField] bool intensityUp;

    // Start is called before the first frame update
    void Start()
    {
        lensFlare = GetComponent<LensFlareComponentSRP>();
    }

    // Update is called once per frame
    void Update()
    {
        if (intensityUp)
        {
            intensity += (float)(Time.deltaTime * 1);
            if (intensity >= 0.6)
            {
                intensityUp = false;
            }
            lensFlare.intensity = intensity < 0.2f ? 0.2f : intensity;
        }
        else
        {
            intensity -= (float)(Time.deltaTime * 1);
            if (intensity <= -1.2)
            {
                intensityUp = true;
            }
            lensFlare.intensity = intensity < 0.2f ? 0.2f : intensity;
        }
    }
}
