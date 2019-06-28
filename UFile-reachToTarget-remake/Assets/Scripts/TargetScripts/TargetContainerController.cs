using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

/*
 * File:    TargetController.cs
 * Project: ReachToTarget-Remake
 * Author:  Peter Caruana
 * York University (c) 2019 
 * Vision Research Labs
 */


public class TargetContainerController : MonoBehaviour
{
    public GameObject targetPrefab;
    public GameObject boxPrefab;
    public GameObject cylinderPrefab;
    public GameObject PhysicsCubePrefab;
    public GameObject PhysicsSpherePrefab;
    public bool IsGrabTrial;
    public float targetRadius = 0.12f;
    public AudioSource audioSource;
    public Session session;
    public ExperimentController experimentController;
    public bool soundActive = true;
    public ParticleSystem particleSystem;

    List<float> shuffledTargetList = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        //Until Game Start is implemented
        //gameObject.SetActive(true);
    }


    public void SpawnTarget(Trial trial)
    {
        SetTargetAngle(trial); //set the target angle for this trial
        

        //the distance to instantiate the target is in the z position

        if (IsGrabTrial)
        {
            targetRadius = 0.3f;
            string objectType = trial.settings.GetString("object_type");
            if (objectType == "cube")
            {
                var target = Instantiate(PhysicsCubePrefab, transform);
                var box = Instantiate(boxPrefab, transform);
                box.transform.localPosition = new Vector3(0, 0, targetRadius);
                target.transform.localPosition = new Vector3(0, 0, 0.1f);
                Debug.Log("Spawned physics cube []");
            }
            else if (objectType == "sphere")
            {
                var target = Instantiate(PhysicsSpherePrefab, transform);
                var box = Instantiate(cylinderPrefab, transform);
                box.transform.localPosition = new Vector3(0, 0, targetRadius);
                target.transform.localPosition = new Vector3(0, 0, 0.1f);
                Debug.Log("Spawned physics sphere O");
            }
        }
        else
        {
            var target = Instantiate(targetPrefab, transform);
            target.transform.localPosition = new Vector3(0, 0, targetRadius);
            particleSystem.transform.localPosition = new Vector3(0, 0, targetRadius);
            Debug.Log("Target has been spawned at: " + target.transform.localPosition.ToString());
        }
        

        
    }


    public void PlayDestroyNoise()
    {
        audioSource.Play();
        float pitch = Random.Range(0.5f, 1.5f);
        audioSource.pitch = pitch;
        Debug.Log("Audio Clip Played, Pitch: " + pitch);
    }

    private void explodeParticles()
    {
        particleSystem.Play();
       
    }

    public void DestroyBoxes()
    {
        GameObject[] boxes;
        boxes = GameObject.FindGameObjectsWithTag("Box");
        for (var i = 0; i < boxes.Length; i++)
        {
            Destroy(boxes[i]);
        }
    }
    public void DestroyTargets()
    {
        GameObject[] targets;
        if (IsGrabTrial)
        {
            targets = GameObject.FindGameObjectsWithTag("ExperimentObject");
        }
        else {
            targets = GameObject.FindGameObjectsWithTag("Target");
            explodeParticles();
        }
        
        if (soundActive)
        {
            PlayDestroyNoise();
        }
        //gameObject.SetActive(false);
        for (var i = 0; i < targets.Length; i++)
        {
            Destroy(targets[i]);
        }
    }


    void SetTargetAngle(Trial trial)
    {
        float targetLocation = trial.settings.GetFloat("targetAngle"); // set this at trial start
        float targetVPos = trial.settings.GetFloat("target_vertPos");
        //rotate this thing 
        transform.rotation = Quaternion.Euler(targetVPos * -1, targetLocation - 90, 0); //the Y here is WRONG! 
    }


    // run at start of every trial
    public void DetermineTrialTargetAngle(Trial trial)
    {

        //Pseudorandom target location
        if (shuffledTargetList.Count < 1)
        {
            //from this trial, grab which targetlist we want to use
            string targetListToUse = trial.settings.GetString("targetListToUse");

            // from session, grab that targetlist
            List<float> targetList = session.settings.GetFloatList(targetListToUse);

            shuffledTargetList = targetList; //might be redundant but fine

            shuffledTargetList.Shuffle();
        }

        float targetAngle = shuffledTargetList[0];

        //print(targetLocation);
        //remove the used target from the list
        shuffledTargetList.RemoveAt(0);

        trial.settings.SetValue("targetAngle", targetAngle);
    }
}
