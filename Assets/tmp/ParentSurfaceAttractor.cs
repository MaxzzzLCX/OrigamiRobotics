using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentSurfaceAttractor : MonoBehaviour
{
    [SerializeField] LayerMask DetectObjectLayerMask;
    [SerializeField] Transform AffectedTransform;
    [SerializeField] Rigidbody body;
    [SerializeField] float raycastDistance=2f;
    public void ForceMoveToSurface()
    {
        
        ForceMoveToSurface(raycastDistance);
    }

    public void ForceMoveToSurface(float RaycastDistance)
    {if (AffectedTransform == null) { AffectedTransform = transform; }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, RaycastDistance, DetectObjectLayerMask))
        {
            // Oblicz now¹ pozycjê na podstawie trafienia
            Vector3 newPosition = hit.point + (hit.normal * 0.1f)+ AffectedTransform.InverseTransformPoint(this.transform.position);
            AffectedTransform.position = newPosition;
          if(body==null)  body = AffectedTransform.GetComponentInChildren<Rigidbody>();
            if (body != null) { body.velocity = Vector3.zero; body.angularVelocity = Vector3.zero; }
            
        }
    }
}
