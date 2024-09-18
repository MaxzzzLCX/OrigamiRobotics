using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    private Color originalColor;
    public Color hoverColor = Color.red; // You can set this in the Inspector
    private Renderer sphereRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Renderer component from the sphere
        sphereRenderer = GetComponent<Renderer>();
        // Store the original color of the sphere's material
        originalColor = sphereRenderer.material.color;
    }


    public void OnHoverEnter()
    {
        sphereRenderer.material.color = hoverColor;
    }
    public void OnHoverExit()
    {
        sphereRenderer.material.color = originalColor;
    }
}
