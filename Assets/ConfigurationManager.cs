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

    public UnityEvent EndOfExperiment;

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


    public class Configuration
    {
        public string experiment_sequence;
    }
    
    private void LoadJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            config = JsonUtility.FromJson<Configuration>(jsonContent);

            Debug.Log("Json file is read");
            Debug.Log("Sequence" + config.experiment_sequence);
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

            if (config != null && modelIndex + 1 < config.experiment_sequence.Length)
            {
                if (timeLoggerManager != null)
                {
                    Debug.Log("[LOG] timeLoggerManager is not null");
                    timeLoggerManager.endOfModel(modelIndex, config.experiment_sequence[modelIndex]);
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
                EndOfExperiment.Invoke();
            }
        }
    }

    public void StartOfModel()
    {
        timeLoggerManager.startOfModel();
    }

    // This function is used to set the flag 'ReturnedToMenu' as explained above in variable declaration. 
    public void SetReturnToMenuFlag(bool newFlag)
    {
        ReturnedToMenu = newFlag;
        Debug.Log("[NEWNEW] Set ReturnedToMenu to true");
    }

   
}
