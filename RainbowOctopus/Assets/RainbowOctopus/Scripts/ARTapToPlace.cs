/**
 * Copyright 2016 IBM Corp. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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

	public GameObject placementIndicator;

    private Vector2 screenCenterPosition;

	static List<ARRaycastHit> hits = new List<ARRaycastHit>();

	bool ignoreTouchMoves = false;  

	bool initialPlacementComplete = false;


	private void Start()
	{
		targetObject.SetActive(false);
		placementIndicator.SetActive(false);
		screenCenterPosition = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
	}


	void Update()
	{
		DetectTouches();

		if (!initialPlacementComplete)
		{
			UpdateInitialPlacementIndicator();
		}
	}


    private void DetectTouches()
	{
		if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
			switch (touch.phase)
			{
				case TouchPhase.Began:
					OnTouchBegan(touch);
					break;
				case TouchPhase.Moved:
					OnTouchMoved(touch);
					break;
				case TouchPhase.Ended:
					OnTouchEnded(touch);
					break;
			}

        }
	}


	void OnTouchBegan(Touch touch)
	{
		Vector2 position = initialPlacementComplete ? touch.position : screenCenterPosition;
		bool success = PlaceTargetObjectAtScreenPosition(position);

		if (success && !initialPlacementComplete)
		{
			ignoreTouchMoves = true;
			initialPlacementComplete = true;
		}
	}


	void OnTouchMoved(Touch touch)
    {
		if (ignoreTouchMoves) return;

		PlaceTargetObjectAtScreenPosition(touch.position);
    }


	void OnTouchEnded(Touch touch)
    {
		ignoreTouchMoves = false;
    }


    /// <summary>
    /// Attempts to place the target object in AR space based on screen position.
	/// Returns true of the placement was successful, false otherwise.
    /// </summary>
    /// <returns><c>true</c>, if target object at screen position was placed, <c>false</c> otherwise.</returns>
    /// <param name="screenPosition">Screen position in pixels</param>
	private bool PlaceTargetObjectAtScreenPosition(Vector2 screenPosition)
	{
		Pose hitPose;
        if (RealWorldHitTestPlaneAtScreenPosition(screenPosition, out hitPose))
        {
            // Valid placement position found.

            hitPose = hits[0].pose;
            PlaceTargetObject(hitPose);

			return true;
        }

		return false;
	}


	private void PlaceTargetObject(Pose pose)
	{
		targetObject.SetActive(true);
		targetObject.transform.position = pose.position;

        if (turnObjectTowardsCamera)
        {
            Vector3 facingDirection = (Camera.main.transform.position - targetObject.transform.position);
            facingDirection.y = 0;
            facingDirection.Normalize();
            targetObject.transform.rotation = Quaternion.LookRotation(facingDirection);
        }

		placementIndicator.SetActive(false);
	}


	private void UpdateInitialPlacementIndicator()
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
	bool RealWorldHitTestPlaneAtScreenPosition(Vector3 screenPosition, out Pose hitPose, TrackableType trackableType = TrackableType.Planes)
    {
		if (sessionOrigin.Raycast(screenPosition, hits, trackableType))
        {
            hitPose = hits[0].pose;
            return true;
        }

        hitPose = new Pose();
        return false;
    }
}
