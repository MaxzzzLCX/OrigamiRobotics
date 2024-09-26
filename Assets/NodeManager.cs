using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    private Color originalColor;
    public Color foldHoverColor = Color.red; // You can set this in the Inspector
    public Color moveHoverColor = Color.green;
    private Renderer sphereRenderer;
    private float originalZ;

    public bool foldingMode = true;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Renderer component from the sphere
        sphereRenderer = GetComponent<Renderer>();
        // Store the original color of the sphere's material
        originalColor = sphereRenderer.material.color;
        originalZ = gameObject.transform.position.z;
    }

    private void Update()
    {   
        // To limit the sphere move along the same height of z
        Vector3 pos = gameObject.transform.position;
        pos.z = originalZ;
        gameObject.transform.position = pos;
    }


    public void OnHoverEnter()
    {
        if (foldingMode)
        {
            sphereRenderer.material.color = foldHoverColor;
        }
        else
        {
            sphereRenderer.material.color = moveHoverColor;
        }
    }
    public void OnHoverExit()
    {
        sphereRenderer.material.color = originalColor;
    }

    public void FoldingModeActive()
    {
        foldingMode = true;
    }
    public void FoldingModeDeactivate()
    {
        foldingMode = false;
    }
}
