
// This class exists to animate the octopus GameObject, and is applied to the HitCubeParent GameObject.
// The Update function will move the Octopus if it is supposed to be walking.
// Further work, such as making the Octopus detect the edge of the surface in the real world,
// or falling off of a 'real' table onto the floor in the real world, could be done in here.
// It could 'as is' reasonably be moved into the OctopusLogic class if needed.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusTransform : MonoBehaviour {
    private GameObject hcp;                         // The Octopus' parent GameObject.
    Animator octopusAnimator;
    private bool walking = false;                   // Assume we are not walking to begin with.
	// Use this for initialization
	void Start () {
        hcp = GameObject.Find("HitCubeParent");     // Just for clarity, but this script is attached to the HitCubeParent GameObject.
        octopusAnimator = GameObject.Find("octopus").GetComponent<Animator>(); // Get the Octopus's Animator object.
	}

	// Update is called once per frame
	void Update () {
        if (walking)                                    // if the StartForwardMotion function was called externally, we need to walk the octopus across the plane.
        {
            float f = octopusAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;            // In the animation, the Octopus drags itself across the floor. We don't just want the
                                                                                                // Octopus to glide across... we want it to stop and start based on when the animation is
                                                                                                // doing the 'drag' parts. These parts are between frames 141 and 190, and after frame 260.


            int frameNumber = Mathf.FloorToInt(f * 321) % 321;                                  // The animation is on loop, so we get the current frame number, and mod with 321, the total
                                                                                                // number of frames.
            if ((frameNumber > 141 && frameNumber < 190) || frameNumber > 260)                  // Then, if it's the part we want to 'walk' in
            {
                hcp.transform.position += hcp.transform.forward * Time.deltaTime * -0.1f;       // we move the octopus forward - it will go in the direction it is facing.
            }
        }
	}

    public void StartForwardMotion() {                                                          // Called by the controller to ensure the Octopus starts walking in the direction it faces.
        walking = true;
    }

    public void StopForwardMotion() {                                                           // Called by the controller to ensure the Octopus stops walking in the direction it faces.
        walking = false;
    }

}
