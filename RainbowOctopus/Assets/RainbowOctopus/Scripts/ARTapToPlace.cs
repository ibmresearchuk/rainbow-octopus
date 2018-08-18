using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class ARTapToPlace : MonoBehaviour
{
	public ARSessionOrigin sessionOrigin;

	/// <summary>
	/// The GameObject instance that should be moved into position when an
	/// AR plane is tapped.
	/// </summary>
	public GameObject targetObject;

	/// <summary>
	/// Whether the object should be rotated to face the camera when it is placed.
	/// </summary>
	public bool turnObjectTowardsCamera = true;

	static List<ARRaycastHit> hits = new List<ARRaycastHit>();

	private void Start()
	{
		targetObject.SetActive(false);
	}


	void Update()
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
         
			if (sessionOrigin.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
			{
            
				Pose hitPose = hits[0].pose;

				targetObject.SetActive(true);
				targetObject.transform.position = hitPose.position;

				Debug.Log("Tapped surface at: " + hitPose.position);

				if (turnObjectTowardsCamera)
				{
					Vector3 facingDirection = (Camera.main.transform.position - targetObject.transform.position);
					facingDirection.y = 0;
					facingDirection.Normalize();
					targetObject.transform.rotation = Quaternion.LookRotation(facingDirection);
				}
			}
		}
	}
}
