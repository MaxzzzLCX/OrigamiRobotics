using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ColorEvent : UnityEvent<Color> { }

public class ColorBlock : MonoBehaviour
{
    private Renderer rend;

    public ColorEvent onColorSelected;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Ta metoda powinna by� wywo�ywana przez Unity Event z MRTK
    public void SelectColor()
    {
        if (rend && onColorSelected != null)
        {
            onColorSelected.Invoke(rend.material.color);
        }
    }
}
