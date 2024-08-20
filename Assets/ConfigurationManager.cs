using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;


#if WINDOWS_UWP
using Windows.Storage;
#endif

public class ConfigurationManager : MonoBehaviour
{

    public string _settingsPath;
    public string _filename = "experiment_config.json";
    public Configuration config;

    public int modelIndex = 0;
    public int tutorialIndex = 0;
    public int DL = -1;
    public int modelID = -1;
    

    public UnityEvent EndOfExperiment;
    public UnityEvent EndOfTutorial;

    public bool ReturnedToMenu = true;
    // This is a flag used to keep track whether the user returned to menu after an origami model is finished
    // Without this flag, the user can click the "Previous Arrow" when reaching the last folding stage, and click "Next Arrow" again. This will trigger Next model twice, thus skipping over the next origami model. 

    public TimeLoggerManager timeLoggerManager;


    // Start is called before the first frame update
    void Start()
    {
        #if WINDOWS_UWP
        _settingsPath = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, _filename);
        #else
        _settingsPath = Path.Combine(Application.persistentDataPath, _filename);
        #endif
        

        
        Debug.Log("Path of JSON file: " + _settingsPath);

        if (File.Exists(_settingsPath))
        {
            Debug.Log("Path Exist");
            LoadJson(_settingsPath);
        }
        else
        {
            Debug.Log("JSON file does not exist at path: " + _settingsPath);
        }

       
    }

    /*
    private IEnumerator CopyFileToPersistent(string sourceFile, string destinationPath)
    {
        string sourcePath = Path.Combine(Application.streamingAssetsPath, sourceFile, sourceFile);

        using (UnityWebRequest www = UnityWebRequest.Get(sourcePath))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to copy JSON file: " + www.error);
            }
            else
            {
                File.WriteAllText(destinationPath, www.downloadHandler.text);
                Debug.Log("Copied JSON file to persistent path.");
                LoadJson(destinationPath);
            }
        }
    }
    */

    // class Configuration matches the format of the json file. 
    public class Configuration
    {
        public string id;
        public string tutorial_mode;
        public string tutorial_sequence;
        public string experiment_sequence;
    }
    
    // Convert the symbol (a letter A to H) to (1) is DL active and (2) the ID of the model (1, 2, 3, 4, 5)
    private void interpretModelSymbol(char symbol)
    {
        switch (symbol)
        {
            case 'A':
                DL = 1;
                modelID = 2;
                break;
            case 'B':
                DL = 1;
                modelID = 3;
                break;
            case 'C':
                DL = 1;
                modelID = 4;
                break;
            case 'D':
                DL = 1;
                modelID = 5;
                break;
            case 'E':
                DL = 0;
                modelID = 2;
                break;
            case 'F':
                DL = 0;
                modelID = 3;
                break;
            case 'G':
                DL = 0;
                modelID = 4;
                break;
            case 'H':
                DL = 0;
                modelID = 5;
                break;
            default:
                DL = -1;
                modelID = -1;
                break;
        }
        // return (DL, modelID);
    }

    private void LoadJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            config = JsonUtility.FromJson<Configuration>(jsonContent);

            Debug.Log("Json file is read");
            Debug.Log($"Participant ID: {config.id}");
            Debug.Log($"Tutorial status: {config.tutorial_mode}");
            Debug.Log($"Tutorial Sequence: {config.tutorial_sequence}");
            Debug.Log($"Sequence: {config.experiment_sequence}");
        }
        else
        {
            Debug.Log("LoadJSON failed. File does not exist at path" + filePath);
        }
    }

    public void NextModel(GameObject sender) //This method calls the EndOfModel, which writes into time log file
    {
        if (ReturnedToMenu)
        {
            ReturnedToMenu = false; // when ReturnedToMenu is false, the modelIndex will not increment to move onto next origami model to prevent double triggering
                                    // the only way to set ReturnedToMenu to true is to return to the Menu
            // Debug.Log("[NEWNEW] Set ReturnedToMenu to false");

            if (sender != null)
            {
                Debug.Log("[New] NextModel function called by " + sender.name);
            }
            else
            {
                Debug.Log("[New] NextModel called, BUT NO SENDER");
            }


            if (config.tutorial_mode == "0")
            {
                if (config != null && modelIndex + 1 < config.experiment_sequence.Length)
                {
                    if (timeLoggerManager != null)
                    {
                        Debug.Log("[LOG] timeLoggerManager is not null");
                        timeLoggerManager.endOfModel(config.id, DL, modelID);
                    }
                    else
                    {
                        Debug.Log("[LOG] timeLoggerManager is null");
                    }


                    modelIndex += 1;
                    Debug.LogError("Model " + config.experiment_sequence[modelIndex - 1] + "finished. Next model: " + config.experiment_sequence[modelIndex]);
                }
                else
                {
                    Debug.LogError("Experiment finished");
                    timeLoggerManager.endOfModel(config.id, DL, modelID);
                    EndOfExperiment.Invoke();
                }
            }
            else
            {

                if (config != null && tutorialIndex + 1 < config.tutorial_sequence.Length)
                {
                    /*
                    if (timeLoggerManager != null)
                    {
                        Debug.Log("[LOG] timeLoggerManager is not null");
                        // timeLoggerManager.endOfModel(tutorialIndex, config.tutorial_sequence[tutorialIndex]);
                    }
                    else
                    {
                        Debug.Log("[LOG] timeLoggerManager is null");
                    }
                    */

                    tutorialIndex += 1;
                    Debug.LogError("Model " + config.tutorial_sequence[tutorialIndex - 1] + "finished. Next model: " + config.tutorial_sequence[tutorialIndex]);
                }
                else
                {
                    Debug.LogError("Tutorial finished");
                    EndOfTutorial.Invoke();
                }
            }

            
           
        }
    }

    public void StartOfModel()
    {
        interpretModelSymbol(config.experiment_sequence[modelIndex]);
        timeLoggerManager.startOfModel();
    }


    // This function is used to set the flag 'ReturnedToMenu' as explained above in variable declaration. 
    public void SetReturnToMenuFlag(bool newFlag)
    {
        ReturnedToMenu = newFlag;
        Debug.Log("[NEWNEW] Set ReturnedToMenu to true");
    }

   
}
