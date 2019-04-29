using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

public class ExperimentSetup : MonoBehaviour
{
    private void Start()
    {
        // disable the whole task initially to give time for the experimenter to use the UI
        gameObject.SetActive(false);
    }

    public void GenerateExperiment(Session session)
    {
        // retrieve the n_practice_trials setting from the session settings
        int numPracticeTrials = session.settings.GetInt("n_practice_trials");
        // create block 1
        Block practiceBlock = session.CreateBlock(numPracticeTrials);
        practiceBlock.settings.SetValue("practice", true);

        // retrieve the n_main_trials setting from the session settings
        int numMainTrials = session.settings.GetInt("n_main_trials");
        // create block 2
        Block aligned_block1 = session.CreateBlock(numMainTrials); // block 2

        //// here we set a setting for the 2nd trial of the main block as an example.
        //aligned_block1.GetRelativeTrial(2).settings.SetValue("size", 10);
        //aligned_block1.GetRelativeTrial(2).settings.SetValue("color", Color.red);

        // we enable a setting if this is the first session, e.g. to show instructions
        session.GetTrial(1).settings.SetValue("show_instructions", session.number == 1);

        //// we can also do different things depending on participant information
        //int age = Convert.ToInt32(session.participantDetails["age"]);
        //session.settings.SetValue("sensitive_content", age >= 18);
    }

}
