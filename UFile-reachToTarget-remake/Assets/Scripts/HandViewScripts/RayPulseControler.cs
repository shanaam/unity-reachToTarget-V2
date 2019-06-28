using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayPulseControler : MonoBehaviour
{
    public float pulseSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Renderer rend = GetComponent<Renderer>();

        Material mat = rend.material;
        
        float floor = 0.2f;
        float ceiling = 3.0f;
        float emission = floor + Mathf.PingPong(Time.time * pulseSpeed, ceiling - floor);
        Color baseColor = new Color(0.95f,0.95f,0.95f);
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(1/emission);
        mat.SetColor("_EmissionColor", finalColor);
    }
}
