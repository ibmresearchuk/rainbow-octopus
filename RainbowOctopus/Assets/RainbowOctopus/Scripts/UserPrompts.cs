using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class UserPrompts : MonoBehaviour {

	public ARSessionOrigin sessionOrigin;

	public GameObject placementIndicator;

	private Vector3 screenCenterPosition;

	static List<ARRaycastHit> hits = new List<ARRaycastHit>();

	private void Start()
	{
		screenCenterPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
	}

	private void Update()
	{
		Pose hitPose;
		if (RealWorldHitTestPlaneAtScreenPosition(screenCenterPosition, out hitPose))
		{
			placementIndicator.SetActive(true);
			placementIndicator.transform.position = hitPose.position;

			// Rotate to match camera facing direction.
			Vector3 facingDirection = Camera.current.transform.forward;
			facingDirection.y = 0;
			facingDirection.Normalize();
			placementIndicator.transform.rotation = Quaternion.LookRotation(facingDirection);
		}
		else
		{
			placementIndicator.SetActive(false);
		}
	}

	/// <summary>
	/// Tests whether a real world surface (plane) exists at the specified
	/// screen position.
	/// </summary>
	/// <returns><c>true</c>, if a plane exists and outputs the Pose of the hit point.</returns>
	/// <param name="screenPosition">The position on screen to test.</param>
	/// <param name="hitPose">An output parameter to hold the resulting hit pose.</param>
	bool RealWorldHitTestPlaneAtScreenPosition(Vector3 screenPosition, out Pose hitPose)
	{
		if (sessionOrigin.Raycast(screenPosition, hits, TrackableType.PlaneWithinBounds))
		{
			hitPose = hits[0].pose;
			return true;
        }

		hitPose = new Pose();
		return false;
	}
}
