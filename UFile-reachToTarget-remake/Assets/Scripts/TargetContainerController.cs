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
    public float targetRadius = 0.10f;

    public Session session;
    public ExperimentController experimentController;

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
        var target = Instantiate(targetPrefab, transform);
        target.transform.localPosition = new Vector3(0, 0, targetRadius);

        Debug.Log("Target has been spawned");
    }


    public void DestroyTargets()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        gameObject.SetActive(false);
        for (var i = 0; i < targets.Length; i++)
        {
            Destroy(targets[i]);
        }
    }

    void SetTargetAngle(Trial trial)
    {
        float targetLocation = trial.settings.GetFloat("targetAngle"); // set this at trial start
        //rotate this thing 
        transform.rotation = Quaternion.Euler(0, targetLocation - 90, 0);
    }

    // run at start of every trial
    public void DetermineTrialAngle(Trial trial)
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
