using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeAppearance : MonoBehaviour
{

    public bool cornerHover = false;
    public bool sphereHoever = false;
    public GameObject Node;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cornerHover || sphereHoever)
        {
            Node.SetActive(true);
        }
        else
        {
            Node.SetActive(false);
        }
    }

    public void HoverCorner()
    {
        cornerHover = true;
    }
    public void UnHoverCorner()
    {
        cornerHover = false;
    }
    public void HoverSphere()
    {
        sphereHoever = true;
    }
    public void UnHoverSphere()
    {
        sphereHoever = false;
    }

}