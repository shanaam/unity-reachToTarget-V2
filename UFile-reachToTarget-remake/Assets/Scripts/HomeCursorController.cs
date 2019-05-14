using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeCursorController : MonoBehaviour
{
    public bool visible = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Appear()
    {
        Renderer rend = GetComponent<Renderer>();
        rend.enabled = true;
        visible = true;
    }

    public void Remove()
    {
        Renderer rend = GetComponent<Renderer>();
        rend.enabled = false;
        visible = false;
    }
}
