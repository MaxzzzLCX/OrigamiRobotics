using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PaperSegment : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FoldingAnimation(Vector3 midpoint, Vector3 foldAxis)
    {
        float foldingAngle = -180f; // The angle to fold, adjust as needed
        float animationDuration = 2f; // Duration of the folding animation

        StartCoroutine(AnimateFold(gameObject, midpoint, foldAxis, foldingAngle, animationDuration));
    }

    private IEnumerator AnimateFold(GameObject foldingPart, Vector3 axisPoint, Vector3 axisDirection, float totalAngle, float duration)
    {
        float elapsed = 0f;
        float currentAngle = 0f;

        while (elapsed < duration)
        {
            float deltaAngle = (Time.deltaTime / duration) * totalAngle;
            foldingPart.transform.RotateAround(axisPoint, axisDirection, deltaAngle);

            elapsed += Time.deltaTime;
            currentAngle += deltaAngle;
            yield return null;
        }

        // Correct any residual angle due to frame time inaccuracies
        float remainingAngle = totalAngle - currentAngle;
        if (Mathf.Abs(remainingAngle) > 0.01f)
        {
            foldingPart.transform.RotateAround(axisPoint, axisDirection, remainingAngle);
        }
    }
}
