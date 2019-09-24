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
    public float grabObjSpawnDist = 0.15f;

    List<float> shuffledTargetList = new List<float>();
    List<int> shuffledRecepticleList = new List<int>();
    int currentRecepticle = 0;
    int batchSize = 0;
    private GameObject recepticlePrefab;

    // Start is called before the first frame update
    void Start()
    {
        //Until Game Start is implemented
        //gameObject.SetActive(true);
    }


    public void SpawnTarget(Trial trial)
    {
        SetTargetAngle(trial); //set the target angle for this trial

        targetDistance = trial.settings.GetFloat("target_distance"); //set target distance for this trial
        targetDistance = targetDistance / 100;
        //the distance to instantiate the target is in the z position

        if (IsGrabTrial)
        {

            //targetDistance = 0.3f; 
            var grabObject = Instantiate(
                recepticlePrefab.name == boxPrefab.name ? PhysicsCubePrefab : PhysicsSpherePrefab, transform);

            var recepticle = Instantiate(recepticlePrefab, transform);

            recepticle.transform.localPosition = new Vector3(0f, -0.05f, targetDistance);
            grabObject.transform.localPosition = new Vector3(0f, 0f, grabObjSpawnDist);
            grabObject.GetComponent<GrabableObject>().m_controller = experimentController.GetController();

            // 50/50 chance for either 25 degrees left or right of the target angle
            System.Random rand = new System.Random();
            float deviation = rand.Next(2) == 0 ? -25f : 25f;

            // Rotate the entire transform +/- 15 degrees left or right of the target angle
            transform.rotation = Quaternion.Euler(
                trial.settings.GetFloat("target_vertPos") * -1f,
                (trial.settings.GetFloat("targetAngle") * -1f) + 90f + deviation,
                0.0f
            );

            // Instantiate the recepticle opposite of the physics object
            var wrongRecepticle = Instantiate(
                recepticlePrefab.name == boxPrefab.name ? cylinderPrefab : boxPrefab, transform);
            wrongRecepticle.transform.localPosition = new Vector3(0, -0.05f, targetDistance);

            wrongRecepticle.transform.SetParent(null);
            
            // Rotate the entire transform back to the original angle
            transform.rotation = Quaternion.Euler(
                trial.settings.GetFloat("target_vertPos") * -1f,
                (trial.settings.GetFloat("targetAngle") * -1f) + 90f,
                0.0f
            );

            wrongRecepticle.transform.SetParent(transform);
        }
        else
        {
            if (trial.settings.GetString("type") == "localization")
            {
                Debug.Log("Spawning Arc");
                transform.localPosition = Vector3.zero;
                var target = Instantiate(arcPrefab, transform);
                target.GetComponent<ArcController>().GenerateArc(trial);
            }
            else
            {
                transform.localPosition = new Vector3(0, 0, 0); //sets target container to parent 0
                var target = Instantiate(targetPrefab, transform);
                target.transform.localPosition = new Vector3(0, 0, targetDistance);
                particleSystem.transform.localPosition = new Vector3(0, 0, targetDistance);
                Debug.Log("Target has been spawned at: " + target.transform.localPosition.ToString());
            }
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
        transform.rotation = Quaternion.Euler(targetVPos * -1, (targetLocation * -1) + 90, 0); //the Y here is right (fixed) 
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

        // Pseudorandom recepticle location
        if (shuffledRecepticleList.Count < 1)
        {
            int trials = trial.session.CurrentBlock.trials.Count;
            int targets = session.settings.GetFloatList(trial.settings.GetString("targetListToUse")).Count;
            batchSize = targets - 1;

            if (trials % targets != 0)
            {
                Debug.LogError("WARNING! NUMBER OF TARGETS DOES NOT EQUALLY DIVIDE INTO NUMBER OF TRIALS");
            }

            // Number of recepticles per list of targets
            int recepticles = trials / targets;
            for (int i = 0; i < recepticles; i++)
            {
                // 1 is box, 0 is sphere
                shuffledRecepticleList.Add(i % 2 == 0 ? 1 : 0);
            }

            currentRecepticle = batchSize;
            shuffledRecepticleList.Shuffle();
        }

        recepticlePrefab = shuffledRecepticleList[0] == 0 ? cylinderPrefab : boxPrefab;

        if (currentRecepticle > 0) { currentRecepticle--; }
        else
        {
            shuffledRecepticleList.RemoveAt(0);
            currentRecepticle = batchSize;
        }

        float targetAngle = shuffledTargetList[0];

        //print(targetLocation);
        //remove the used target from the list
        shuffledTargetList.RemoveAt(0);

        trial.settings.SetValue("targetAngle", targetAngle);
    }
}
