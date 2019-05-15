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
        List<int> per_block_n = session.settings.GetIntList("per_block_n");
        List<string> per_block_type = session.settings.GetStringList("per_block_type");
        List<string> per_block_targetListToUse = session.settings.GetStringList("per_block_targetListToUse");
        List<float> per_block_rotation = session.settings.GetFloatList("per_block_rotation");
        

        for(int i=0; i < per_block_n.Count; i++)
        {
            session.CreateBlock(per_block_n[i]);
            session.blocks[i].settings.SetValue("type", per_block_type[i]);
            session.blocks[i].settings.SetValue("targetListToUse", per_block_targetListToUse[i]);
            
        }
        
    }
    

    public void TestExperiment(Trial trial)
    {
        Debug.LogFormat("targetYOffset in Controller set to {0}", trial.settings.GetObject("type"));
    }

}
