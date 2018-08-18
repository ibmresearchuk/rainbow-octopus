using System;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEngine;
using UnityEngine.UI;


public class OctopusLogic : MonoBehaviour {
    public GameObject octopus, octopusTransformParent, debugPanel;
    private OctopusTransform c;
    Animator octopusAnimator;

	// Use this for initialization
	void Start () {
        c = octopusTransformParent.GetComponent<OctopusTransform>();                           // the transform object of the parent.
        octopusAnimator = octopus.GetComponent<Animator>();
	}

    // perform the logic on the octopus. In our case this is only called from the ConversationHandler, but could be called from other controllers.
	public void processIntent (string intent) {
        Log.Debug("OctopusLogic.processIntent()", "Intent: {0}", intent);
        switch (intent.ToLower())
        {
            case ("wave") : {
                    Log.Debug("OctopusLogic.processIntent()", "Setting Octopus to Wave");
                    octopusAnimator.CrossFade("Wave", 0);                   // With a crossFade of >0, it seems to take too long to respond to the command.
                    c.StopForwardMotion();
                    break;
                }
            case ("walk") : {
                    Log.Debug("OctopusLogic.processIntent()", "Setting Octopus to Walk");
                    octopusAnimator.CrossFade("Walk", 0);                   // With a crossFade of >0, it seems to take too long to respond to the command.
                    c.StartForwardMotion();
                    break;
                }
            case ("jump") : {
                Log.Debug("OctopusLogic.processIntent()", "Setting Octopus to Jump");
                    octopusAnimator.CrossFade("Jump", 0);                       // With a crossFade of >0, it seems to take too long to respond to the command.
                    c.StopForwardMotion();
                break;
            }
            case ("bigger") : {
                    Log.Debug("OctopusLogic.processIntent()", "Increasing Octopus size");
                    octopus.transform.localScale = new Vector3(octopus.transform.localScale.x * 1.2f, octopus.transform.localScale.y * 1.2f, octopus.transform.localScale.z * 1.2f);
                    break;
                }

            case ("smaller") : {
                    Log.Debug("OctopusLogic.processIntent()", "Decreasing Octopus size");
                    octopus.transform.localScale = new Vector3(octopus.transform.localScale.x * 0.8f, octopus.transform.localScale.y * 0.8f, octopus.transform.localScale.z * 0.8f);
                    break;
                }

            case ("clockwise"):
                {
                    Log.Debug("OctopusLogic.processIntent()", "Turning 90 degrees Clockwise");
                    octopusTransformParent.transform.Rotate(0, 90, 0);
                    break;
                }

            case ("anticlockwise"):
                {
                    Log.Debug("OctopusLogic.processIntent()", "Turning 90 degrees Anticlockwise");
                    octopusTransformParent.transform.Rotate(0, -90, 0);
                    break;
                }
            case ("debug") : {
                    Log.Debug("OctopusLogic.processIntent()", "Toggling the debug window");
                    //if (debugPanel.alpha == 1f) debugPanel.alpha = 0f;
                    //else debugPanel.alpha = 1f;
                    if (debugPanel.active) debugPanel.SetActive(false);
                    else debugPanel.SetActive(true);
                    break;
                }

            case ("idle"):
                {
                    Log.Debug("OctopusLogic.processIntent()", "Setting Octopus to idle");
                    octopusAnimator.CrossFade("Idle1", 0);
                    c.StopForwardMotion();
                    break;
                }
            default:
                break;
        }

	}
}
