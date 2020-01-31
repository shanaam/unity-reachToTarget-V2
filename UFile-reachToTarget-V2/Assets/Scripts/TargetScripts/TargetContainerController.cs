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
    public GameObject secondaryHomePrefab;
    public GameObject arcPrefab;
    public GameObject boxPrefab;
    public GameObject cylinderPrefab;
    public GameObject PhysicsCubePrefab;
    public GameObject PhysicsSpherePrefab;
    public bool IsGrabTrial;
    public float targetDistance = 12f;
    public AudioSource audioSource;
    public Session session;
    public ExperimentController experimentController;
    public bool soundActive = true;
    public ParticleSystem particleSystem;

    // variable
    public float grabObjSpawnDist = 0.05f;
    public float secondHomeDist = 0.02f;

    List<float> shuffledTargetList = new List<float>();
    List<int> shuffledRecepticleList = new List<int>();
    int currentRecepticle = 0;
    int batchSize = 0;
    public GameObject receptaclePrefab;

    // Start is called before the first frame update
    void Start()
    {
        //Until Game Start is implemented
        //gameObject.SetActive(true);
    }


    public void SpawnTarget(Trial trial)
    {
        float targetAngle = trial.settings.GetFloat("targetAngle");
        float vertPos = trial.settings.GetFloat("target_vertPos");

        SetTargetAngle(targetAngle, vertPos); //set the target angle for this trial

        targetDistance = trial.settings.GetFloat("target_distance"); //set target distance for this trial
        targetDistance = targetDistance / 100.0f;
        //the distance to instantiate the target is in the z position

        if (IsGrabTrial)
        {
            // Reset the angle to 90 degrees (forward)
            SetTargetAngle(90f, vertPos);

            // Offset the entire target container
            transform.localPosition = new Vector3(0.0f, 0.0f, grabObjSpawnDist);
            //targetDistance = 0.3f; 

            var grabObject = Instantiate(
                receptaclePrefab.name == boxPrefab.name ? PhysicsCubePrefab : PhysicsSpherePrefab, transform);

            grabObject.transform.localPosition = new Vector3(0f, 0.05f, 0.0f); //drops from here
            grabObject.GetComponent<GrabableObject>().m_controller = experimentController.GetController();
            grabObject.transform.SetParent(null);


            Vector3 receptacleLocation = new Vector3(0f, 0f, targetDistance - grabObjSpawnDist);

            // 50/50 chance for either 25 degrees left or right of the target angle
            System.Random rand = new System.Random();

            float deviation = 180f; 

            switch (targetAngle)
            {
                case 45f:
                    deviation = (rand.Next(2) == 0 ? 90f : 135f);
                    break;
                case 90f:
                    deviation = (rand.Next(2) == 0 ? 45f : 135f);
                    break;
                case 135f:
                    deviation = (rand.Next(2) == 0 ? 45f : 90f);
                    break;
            }
            
            experimentController.distractorLoc = -deviation; //is this necessary?

            // Set up receptacle
            SetTargetAngle(targetAngle, vertPos);
            var receptacle = Instantiate(receptaclePrefab, transform);
            receptacle.transform.localPosition = receptacleLocation;
            receptacle.transform.SetParent(null);

            
            // Set up incorrect receptacle
            SetTargetAngle(deviation, vertPos);
            var wrongReceptacle = Instantiate(
                receptaclePrefab.name == boxPrefab.name ? cylinderPrefab : boxPrefab, transform);
            wrongReceptacle.transform.localPosition = receptacleLocation;
            wrongReceptacle.transform.SetParent(null);


            // Reset the position of the target container back to its original position
            SetTargetAngle(targetAngle, vertPos);

            transform.localPosition = Vector3.zero;

            grabObject.transform.SetParent(transform);
            wrongReceptacle.transform.SetParent(transform);
            receptacle.transform.SetParent(transform);

            //log some things
            experimentController.objSpawnX = grabObject.transform.position.x;
            experimentController.objSpawnZ = grabObject.transform.position.z;

            // log receptacle too
            experimentController.recepticleX = receptacle.transform.position.x;
            experimentController.recepticleY = receptacle.transform.position.y;
            experimentController.recepticleZ = receptacle.transform.position.z;
        }
        else
        {
            // Set target angle to forwards
            SetTargetAngle(90f, vertPos);
            transform.localPosition = new Vector3(0, 0, secondHomeDist); //sets target container to parent 0

            // Spawn the second home
            var secondHome = Instantiate(secondaryHomePrefab, transform);
            secondHome.transform.SetParent(null);

            // Set up the real target
            SetTargetAngle(targetAngle, vertPos);

            // Set up the real target
            var target = Instantiate(
                trial.settings.GetString("type") == "localization" ? arcPrefab : targetPrefab, 
                transform
            );

            if (trial.settings.GetString("type") == "localization")
            {
                target.GetComponent<ArcController>().GenerateArc(trial);
                target.GetComponent<ArcController>().SecondaryHomePos = secondHome.transform.position;
            }
            else
            {
                target.transform.localPosition = new Vector3(0, 0, targetDistance);
                target.GetComponent<TargetController>().SecondaryHomePos = secondHome.transform.position;
                
            }

            target.transform.SetParent(null);

            // Assign reference of the real target to the second home
            secondHome.GetComponent<TargetController>().IsSecondaryHome = true;
            secondHome.GetComponent<TargetController>().RealTarget = target;

            // Reset all positions and re-parent everything
            transform.localPosition = Vector3.zero;
            secondHome.transform.SetParent(transform);
            target.transform.SetParent(transform);
            particleSystem.transform.localPosition = target.transform.localPosition;

            target.SetActive(false);
        }
    }


    public void PlayDestroyNoise()
    {
        audioSource.Play();
        float pitch = Random.Range(0.5f, 1.5f);
        audioSource.pitch = pitch;
        //Debug.Log("Audio Clip Played, Pitch: " + pitch);
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


    /// <summary>
    /// Rotates the entire container to the specified angle
    /// </summary>
    void SetTargetAngle(float angle, float vertical)
    {
        transform.rotation = Quaternion.Euler(vertical * -1, (angle * -1) + 90, 0); //the Y here is right (fixed) 
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

        if (trial.settings.GetString("experiment_mode") == "objectToBox")
        {
            // Pseudorandom receptacle location
            if (shuffledRecepticleList.Count < 1)
            {
                int trials = trial.session.CurrentBlock.trials.Count;
                int targets = session.settings.GetFloatList(trial.settings.GetString("targetListToUse")).Count;
                batchSize = targets - 1;

                if (trials % targets != 0)
                {
                    Debug.LogError("WARNING! NUMBER OF TARGETS DOES NOT EQUALLY DIVIDE INTO NUMBER OF TRIALS");
                }

                // Number of receptacles per list of targets
                int receptacles = trials / targets;
                for (int i = 0; i < receptacles; i++)
                {
                    // 1 is box, 0 is sphere
                    // Can be randomized or set to have all spheres or cubes per block
                    switch (trial.settings.GetString("obj_type"))
                    {
                        case "sphere":
                            shuffledRecepticleList.Add(0);
                            break;
                        case "cube":
                            shuffledRecepticleList.Add(1);
                            break;
                        case "random":
                        default:
                            shuffledRecepticleList.Add(i % 2 == 0 ? 1 : 0);
                            break;
                    }
                }

                currentRecepticle = batchSize;
                shuffledRecepticleList.Shuffle();
            }

            receptaclePrefab = shuffledRecepticleList[0] == 0 ? cylinderPrefab : boxPrefab;

            if (currentRecepticle > 0) { currentRecepticle--; }
            else
            {
                shuffledRecepticleList.RemoveAt(0);
                currentRecepticle = batchSize;
            }
        }

        float targetAngle = shuffledTargetList[0];

        //print(targetLocation);
        //remove the used target from the list
        shuffledTargetList.RemoveAt(0);

        trial.settings.SetValue("targetAngle", targetAngle);
    }
}
