using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;


/*
 * File: ExperimentSetup.cs
 * License: York University (c) 2019
 * Author: Peter Caruana
 * Desc:    Loads json file with experiment description and generates list of blocks upon start of experiment. 
 */

public class ExperimentSetup : MonoBehaviour
{
    private void Start()
    {
        // disable the whole task initially to give time for the experimenter to use the UI
        gameObject.SetActive(false);
    }


    public void GenerateBlocks(Session session)
    {
        List<int> blocks_n = session.settings.GetIntList("blocks_n");
        List<string> blocks_type = session.settings.GetStringList("blocks_type");
        List<int> blocks_targetList = session.settings.GetIntList("blocks_targetList");
        Dictionary<string, object> targetList = session.settings.GetDict("targetList");

        

        for(int i=0; i < blocks_n.Count; i++)
        {
            session.CreateBlock(blocks_n[i]);
            session.blocks[i].settings.SetValue("type", blocks_type[i]);
            session.blocks[i].settings.SetValue("targetList", blocks_targetList[i]);
            
        }
        
    }
    

    public void TestExperiment(Trial trial)
    {
        Debug.LogFormat("targetYOffset in Controller set to {0}", trial.settings.GetObject("type"));
    }

}
