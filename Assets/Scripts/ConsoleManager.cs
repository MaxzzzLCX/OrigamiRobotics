using System.Collections.Generic;
using UnityEngine;
using TMPro; // Pamiêtaj o dodaniu tej przestrzeni nazw

public class ConsoleManager : MonoBehaviour
{
    public TMP_InputField inputField; // Pole do wprowadzania komend
    public TMP_Text consoleOutput; // Obszar tekstowy do wyœwietlania odpowiedzi

    private List<string> commandHistory = new List<string>();
    private int historyIndex = -1;

    private void Start()
    {
        if (inputField != null)
        {
            inputField.onSubmit.AddListener(delegate { SubmitCommand(inputField.text); });
        }
    }

    private void SubmitCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;

        commandHistory.Add(command);
        historyIndex = commandHistory.Count;

        // Tutaj mo¿esz dodaæ w³asn¹ logikê obs³ugi komend
        // Na razie po prostu wyœwietlamy wprowadzon¹ komendê
        consoleOutput.text += "\n" + command;

        inputField.text = ""; // Czyœci pole wprowadzania po wprowadzeniu komendy
        inputField.ActivateInputField(); // Ponownie aktywuje pole wprowadzania
    }

    private void Update()
    {
        if (inputField.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                NavigateHistory(-1); // Przesuñ w górê po historii
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                NavigateHistory(1); // Przesuñ w dó³ po historii
            }
        }
    }

    private void NavigateHistory(int direction)
    {
        if (commandHistory.Count == 0) return;

        historyIndex += direction;

        historyIndex = Mathf.Clamp(historyIndex, 0, commandHistory.Count - 1);

        inputField.text = commandHistory[historyIndex];
        inputField.caretPosition = inputField.text.Length; // Przesuñ kursor na koniec tekstu
    }
}
