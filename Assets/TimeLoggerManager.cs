using OpenCVForUnity.Features2dModule;
using OpenCVForUnity.TrackingModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TimeLoggerManager: MonoBehaviour
{
    // Start is called before the first frame update
    public ExperimentHandler experimenHandler;
    public DateTime timeStartOfModel;
    public DateTime timeStartOfStep;
    public DateTime timeEndOfModel;
    public DateTime timeEndOfStep;
    public TimeSpan durationModel;
    public TimeSpan durationStep;
    


    void Start()
    {
        experimenHandler = new ExperimentHandler();
    }

    public void startOfModel() // When the <STEP> object of origami model appears, this function is called
    {
        timeStartOfModel = DateTime.Now;
    }
    public void endOfModel(string id, int DL, int modelID) 
    {
        timeEndOfModel = DateTime.Now;
        durationModel = timeEndOfModel - timeStartOfModel;

        string message = $"{id}, {DL}, {modelID}, {"-1"}, {durationModel}";
        // string message = $"({modelIndex}) Model {modelSymbol} is finished, duratation: {durationModel}";
        experimenHandler.LogMessage(message, true); //true meaning writing log into time_log.txt
    }


    public void startOfStep()
    {
        timeStartOfStep = DateTime.Now;
    }
   
    public void endOfStep(string id, int DL, int modelID, int stepIndex)
    {
        timeEndOfStep = DateTime.Now;
        durationStep = timeEndOfStep - timeStartOfStep;
        string message = $"{id}, {DL}, {modelID}, {stepIndex}, {durationStep}";
        experimenHandler.LogMessage(message, true); //true meaning writing log into time_log.txt
    }

}
