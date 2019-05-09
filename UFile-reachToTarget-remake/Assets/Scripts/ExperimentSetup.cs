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
        List<int> blocks_n = session.settings.GetIntList("blocks_n");

        // retrieve the n_practice_trials setting from the session settings
        int numPracticeTrials = blocks_n[0];
        // create block 1
        Block practiceBlock = session.CreateBlock(numPracticeTrials);
        practiceBlock.settings.SetValue("type", "practice");

        // retrieve the n_main_trials setting from the session settings
        int numMainTrials = blocks_n[1];
        // create block 2
        Block aligned_block1 = session.CreateBlock(numMainTrials); // block 2
        aligned_block1.settings.SetValue("type", "aligned");

        //// here we set a setting for the 2nd trial of the main block as an example.
        //aligned_block1.GetRelativeTrial(2).settings.SetValue("size", 10);
        //aligned_block1.GetRelativeTrial(2).settings.SetValue("color", Color.red);

        // we enable a setting if this is the first session, e.g. to show instructions
        session.GetTrial(1).settings.SetValue("show_instructions", session.number == 1);

        //// we can also do different things depending on participant information
        //int age = Convert.ToInt32(session.participantDetails["age"]);
        //session.settings.SetValue("sensitive_content", age >= 18);
    }

    public void GenerateBlocks(Session session)
    {
        List<int> blocks_n = session.settings.GetIntList("blocks_n");
        List<object> blocks_type = session.settings.GetObjectList("blocks_type");
        List<int> blocks_targetList = session.settings.GetIntList("blocks_targetList");
        Dictionary<String, object> targetList = session.settings.GetDict("targetList");

        List<Block> blockList = new List<Block>();

        for(int i=0; i < blocks_n.Count; i++)
        {
            blockList[i] = session.CreateBlock(blocks_n[i]);
            blockList[i].settings.SetValue("type", blocks_type[i]);
            blockList[i].settings.SetValue("targetList", blocks_targetList[i]);
            blockList[i].settings.SetValue("targetList", targetList[blocks_targetList[i].ToString()]);
        }

        session.blocks = blockList;
        //return blockList;
        
    }
    

    public void TestExperiment(Trial trial)
    {
        Debug.LogFormat("targetYOffset in Controller set to {0}", trial.settings.GetObject("type"));
    }

}
