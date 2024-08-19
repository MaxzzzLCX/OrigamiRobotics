using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

#if WINDOWS_UWP
using Windows.Storage;
using Windows.Storage.Streams;
#endif

public class ExperimentHandler
{
    public string expFolderName;
#if WINDOWS_UWP
    private Windows.Storage.StorageFile logFile;
    private Windows.Storage.StorageFile logTimeFile;
    private Windows.Storage.StorageFolder expFolder;
    private Windows.Storage.StorageFolder storageFolder;
#endif

    public ExperimentHandler()
    {
        expFolderName = $"exp_{System.DateTime.Now.ToString("yyyy-dd-MM-HH-mm-ss")}";
        // expFolderName = folderName;

#if WINDOWS_UWP
        storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        Task task = new Task(
            async () =>
            {
                expFolder = await storageFolder.CreateFolderAsync(expFolderName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            }
            );
        task.Start();
        task.Wait();
#endif
    }

    public void LogMessage(string message, bool timeRelatedLog = false)
    {
        var currentTime = System.DateTime.Now.Ticks;
        var line = $"{currentTime}, {message} \n";
        if (timeRelatedLog) // Write into time log file
        {
            LogTime(line);
        }
        else // Write into validation related log file
        {
            Log(line);
        }
    }

    private async void Log(string data)
    {
#if WINDOWS_UWP
        if(logFile == null){
            logFile = await expFolder.CreateFileAsync("experiment_log.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
        }
        Task task = new Task(
        async () =>
            {
                await Windows.Storage.FileIO.AppendTextAsync(logFile, data);
            }
        );
        task.Start();
        task.Wait();
#endif   
    }

    private async void LogTime(string data)
    {
#if WINDOWS_UWP
        if(logTimeFile == null){
            logTimeFile = await expFolder.CreateFileAsync("time_log.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
        }
        Task task = new Task(
        async () =>
            {
                await Windows.Storage.FileIO.AppendTextAsync(logTimeFile, data);
            }
        );
        task.Start();
        task.Wait();
#endif   
    }

}
