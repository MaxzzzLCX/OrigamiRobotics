using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceWorldScale : MonoBehaviour
{
    [SerializeField] Vector3 scaleToSet= new Vector3(1,1,1);
    public void ForceWScale()
    {
        Vector3 worldScaleStart = this.transform.lossyScale;
        Vector3 cs = this.transform.localScale;
        this.transform.localScale =new Vector3( (cs.x/ worldScaleStart.x)* scaleToSet.x, (cs.y/worldScaleStart.y)* scaleToSet.y, (cs.z/worldScaleStart.z)*scaleToSet.z);
    }

}
