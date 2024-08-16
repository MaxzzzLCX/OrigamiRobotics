using OpenCVForUnity.Features2dModule;
using OpenCVForUnity.TrackingModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TimeLoggerManager : MonoBehaviour
{
    // Start is called before the first frame update
    public NeuralNetworkController neuralNetworkController;
    public string timeStartOfModel;
    public string timeStartOfStep;
    public string timeEndOfModel;
    public string timeEndOfStep;


    void Start()
    {
       
    }

    public void startOfModel()
    {
        timeStartOfModel = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
}
