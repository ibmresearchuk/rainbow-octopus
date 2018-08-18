using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class PlaceOnPlane : MonoBehaviour
{
    public ARSessionOrigin sessionOrigin;
    
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (sessionOrigin.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                transform.position = hitPose.position;
            }
        }
    }
}
