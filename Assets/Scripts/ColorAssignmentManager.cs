using UnityEngine;

public class ColorAssignmentManager : MonoBehaviour
{
    public ColorBlock[] colorBlocks; // przypisz wszystkie bloczki kolor�w w inspektorze
    public Material targetMaterial; // przypisz docelowy materia� w inspektorze

    public void AssignColor(Color color)
    {
        targetMaterial.color = color;
    }

    private void Start()
    {
        foreach (var block in colorBlocks)
        {
            block.onColorSelected.AddListener(AssignColor);
        }
    }
}
