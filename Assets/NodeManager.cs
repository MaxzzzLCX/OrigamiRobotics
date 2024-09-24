using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    private Color originalColor;
    public Color hoverColor = Color.red; // You can set this in the Inspector
    private Renderer sphereRenderer;
    private float originalZ;

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
        sphereRenderer.material.color = hoverColor;
    }
    public void OnHoverExit()
    {
        sphereRenderer.material.color = originalColor;
    }
}
