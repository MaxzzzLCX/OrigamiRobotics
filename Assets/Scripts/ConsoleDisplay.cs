using UnityEngine;
using TMPro;

public class ConsoleDisplay : MonoBehaviour
{
    public static ConsoleDisplay Instance { get; private set; } // Statyczna instancja

    public TMP_Text consoleOutput; // Obszar tekstowy do wyœwietlania logów

    private void Awake()
    {
        // Ustawienie singletona
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        consoleOutput.text += logString + "\n";
    }

    public void ClearConsole()
    {
        consoleOutput.text = ""; // Czyœci tekst w konsoli
    }

}
