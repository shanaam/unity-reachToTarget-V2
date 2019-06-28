using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeContainerController : MonoBehaviour
{
    /*
     * ContainerTargetController.cs
     * Peter Caruana, York University (c) 2019
     * Vision Lab
     * Controls targets which are container types
     */
    
    public Collider targetArea;
    public ExperimentController experimentController;
    public HandCursorController handCursorController;
    public bool acceptSphere;
    public bool acceptCube;
    public ParticleSystem particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        handCursorController = GameObject.FindGameObjectWithTag("Cursor").GetComponent<HandCursorController>();
        experimentController = handCursorController.experimentController;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        string objectTag = other.tag;

        if (objectTag == "ExperimentObject")
        {
            if (other.name.Contains("Sphere") && acceptSphere)
            {
                acceptTarget(other); 
            }
            else if (other.name.Contains("Cube") && acceptCube)
            {
                acceptTarget(other);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        string objectTag = other.tag;

        if (objectTag == "ExperimentObject")
        {
            if (other.name.Contains("Sphere") && acceptSphere)
            {
                acceptTarget(other);
            }
            else if (other.name.Contains("Cube") && acceptCube)
            {
                acceptTarget(other);
            }
        }
    }

    private void acceptTarget(Collider other)
    {
        bool isGrabbed = other.GetComponent<GrabableObject>().objectGrabbed;
        if (!isGrabbed)
        {
            explodeParticles();
            Debug.Log("Ding! You put the target in the box!");
            handCursorController.taskCompleted = true;
            experimentController.EndAndPrepare(); //disabled for testing, enable for actual experiment use
        }
    }

    private void explodeParticles()
    {
        particleSystem.Play();

    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        string objectTag = collision.collider.tag;

        if (objectTag == "ExperimentObject")
        {
            //temporary reward notification
            bool isGrabbed = collision.collider.GetComponent<GrabableObject>().objectGrabbed;
            if (!isGrabbed)
            {
                Debug.Log("Ding! You put the target in the box!");
                handCursorController.taskCompleted = true;
                //experimentController.EndAndPrepare(); //disabled for testing, enable for actual experiment use
            }
        }
    } */
}
