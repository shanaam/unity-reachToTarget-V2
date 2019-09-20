using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;


/*
 * File: ExperimentSetup.cs
 * License: York University (c) 2019
 * Author: Shanaathanan Modchalingam, Peter Caruana
 * Desc:    Loads json file with experiment description and generates list of blocks upon start of experiment. 
 */

public class ExperimentSetup : MonoBehaviour
{
    private void Start()
    {
        // disable the whole task initially to give time for the experimenter to use the UI
        //gameObject.SetActive(false);
    }

    //Generates experiment blocks automated based off of json arrays.
    public void GenerateBlocks(Session session)
    {
        List<int> per_block_n = session.settings.GetIntList("per_block_n");
        List<string> per_block_type = session.settings.GetStringList("per_block_type");
        List<string> per_block_targetListToUse = session.settings.GetStringList("per_block_targetListToUse");
        List<float> per_block_rotation = session.settings.GetFloatList("per_block_rotation");
        List<float> per_block_distance = session.settings.GetFloatList("per_block_distance");
        List<float> per_block_target_vertPos = session.settings.GetFloatList("per_block_target_vertPos");
        List<string> per_block_plane = session.settings.GetStringList("per_block_plane");
        List<string> per_block_hand = session.settings.GetStringList("per_block_hand");
        string experiment_mode = session.settings.GetString("experiment_mode");



        for (int i=0; i < per_block_n.Count; i++)
        {
            session.CreateBlock(per_block_n[i]);
            session.blocks[i].settings.SetValue("experiment_mode", experiment_mode);
            session.blocks[i].settings.SetValue("type", per_block_type[i]);
            session.blocks[i].settings.SetValue("targetListToUse", per_block_targetListToUse[i]);
            session.blocks[i].settings.SetValue("cursor_rotation", per_block_rotation[i]);
            session.blocks[i].settings.SetValue("target_distance", per_block_distance[i]);
            session.blocks[i].settings.SetValue("target_vertPos", per_block_target_vertPos[i]);
            session.blocks[i].settings.SetValue("plane_setting", per_block_plane[i]);
            session.blocks[i].settings.SetValue("hand", per_block_hand[i]);
            
            
        }

    }

}
