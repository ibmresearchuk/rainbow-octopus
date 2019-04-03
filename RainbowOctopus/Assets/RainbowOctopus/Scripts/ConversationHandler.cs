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

using System;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.Collections;
using FullSerializer;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using UnityEngine.UI;

// This class has been adapted from the example conversation handler, which is part of the
// Watson Unity SDK. Rather that running examples, it has an AskQuestion function which
// can be called from the SpeechToText Handler, or other handlers if required.

public class ConversationHandler : MonoBehaviour
{
	#region PLEASE SET THESE VARIABLES IN THE INSPECTOR
	[Space(10)]
	[Tooltip("Text field to display the results of streaming.")]
	public Text ResultsField;
	[Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/conversation/api\"")]
	[SerializeField]
	private string _serviceUrl;
	[Tooltip("The workspaceId to run the example.")]
	[SerializeField]
	private string _workspaceId = "xxx";
	[Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
	[SerializeField]
	private string _versionDate = "2017-05-25";
	[Header("CF Authentication")]
	[Tooltip("The authentication username.")]
	[SerializeField]
	private string _username = "xxx";
	[Tooltip("The authentication password.")]
	[SerializeField]
	private string _password = "xxx";
	[Header("IAM Authentication")]
	[Tooltip("The IAM apikey.")]
	[SerializeField]
	private string _iamApikey;
	[Tooltip("The IAM url used to authenticate the apikey (optional). This defaults to \"https://iam.bluemix.net/identity/token\".")]
	[SerializeField]
	private string _iamUrl;
	#endregion


	private static Conversation _service;

	private Dictionary<string, object> _context = null;
	private Text Intenta1;                              // Previous two debug fields for Intent. This approach is OK as we only want 3 lines of debug appearing.
	private Text Intenta2;                              // Previous two debug fields for Intent. This approach is OK as we only want 3 lines of debug appearing.


	void Start()
	{
		LogSystem.InstallDefaultReactors();
		Runnable.Run(CreateService());
		Intenta1 = GameObject.Find("Intenta1").GetComponent<Text>();            // Inititialise the two intent debug fields
		Intenta2 = GameObject.Find("Intenta2").GetComponent<Text>();
	}

	// Function unchanged from the Watson Unity SDK example (except we have commented out
	// the running of the examples.
	private IEnumerator CreateService()
	{
		//  Create credential and instantiate service
		Credentials credentials = null;
		if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
		{
			//  Authenticate using username and password
			credentials = new Credentials(_username, _password, _serviceUrl);
		}
		else if (!string.IsNullOrEmpty(_iamApikey))
		{
			//  Authenticate using iamApikey
			TokenOptions tokenOptions = new TokenOptions()
			{
				IamApiKey = _iamApikey,
				IamUrl = _iamUrl
			};

			credentials = new Credentials(tokenOptions, _serviceUrl);

			//  Wait for tokendata
			while (!credentials.HasIamTokenData())
				yield return null;
		}
		else
		{
			throw new WatsonException("Please provide either username and password or IAM apikey to authenticate the service.");
		}

		_service = new Conversation(credentials);
		_service.VersionDate = _versionDate;

		//        Runnable.Run(Examples());
	}


	// Function added so SpeechToText or other handler can ask questions which impact the Octopus
	// The content of the function comes from the Watson Unity SDK Example.
	public void AskQuestion(string question)
	{

		MessageRequest messageRequest = new MessageRequest()
		{
			input = new Dictionary<string, object>()
			{
				{ "text", question }
			},
			context = _context
		};

		if (!_service.Message(OnMessage, OnFail, _workspaceId, messageRequest))
			Log.Debug("ConversationHandler.AskQuestion()", "Failed to message!");
	}

	// When we receive an intent....
	private void OnMessage(object resp, Dictionary<string, object> customData)
	{
		Log.Debug("ConversationHandler.OnMessage()", "Conversation: Message Response: {0}", customData["json"].ToString());

		// Conversation: Example Message Response:
		//    {
		//        "intents":[
		//            {"intent":"idle","confidence":0.6780582904815674
		//            }
		//        ],
		//        "entities":[
		//        ],
		//        "input":{"text":"stop walking "},
		//        "output":{
		//            "text":["I didn't understand can you try again"],
		//            "nodes_visited":["node_2_1467831978407"],
		//            "log_messages":[]
		//        },
		//        "context":{
		//            "conversation_id":"e697dcd1-bd27-4f51-814e-a1ad083c5ef4",
		//            "system":{
		//                "dialog_stack":[
		//                    {"dialog_node":"root"}
		//                ],
		//                "dialog_turn_counter":1,
		//                "dialog_request_counter":1,
		//                "_node_output_map":{
		//                    "node_2_1467831978407":[0]
		//                },
		//                "branch_exited":true,
		//                "branch_exited_reason":"completed"}
		//                },
		//        "alternate_intents":false
		//    }
		//}


		object _tempIntents = null;
		(resp as Dictionary<string, object>).TryGetValue("intents", out _tempIntents);
		List<object> o = (List<object>)_tempIntents;
		//    Log.Debug("ConversationHandler.OnMessage()", "Intents: {0}", o[0].ToString());

		object _tempIntent = null;
		try
		{
			(o[0] as Dictionary<string, object>).TryGetValue("intent", out _tempIntent);
		}
		catch (Exception)
		{
			_tempIntent = "";
		}
		Intenta2.text = Intenta1.text;                              // we move the existing intents up the archive intent fields in the debug panel
		Intenta1.text = ResultsField.text;                          // we move the existing intents up the archive intent fields in the debug panel
		ResultsField.text = "#" + _tempIntent.ToString();           // and update the current intent field with the new Intent.
		OctopusController octopusLogic = gameObject.GetComponent<OctopusController>();        // We then ask the OctopusLogic class to
		octopusLogic.processIntent(_tempIntent.ToString());                         // process the new Intent and update the appropriate GameObjects.

		Log.Debug("ConversationHandler.OnMessage()", "Intent: {0}", _tempIntent);



		// We don't need the Octopus to use the response from Watson Assistant in our conversation
		// but if we did, we could use this bit of code from the Watson Unity SDK Example:

		//fsData fsdata = null;
		//fsResult r = _serializer.TrySerialize(resp.GetType(), resp, out fsdata);
		//if (!r.Succeeded)
		//    throw new WatsonException(r.FormattedMessages);

		////  Convert fsdata to MessageResponse
		//MessageResponse messageResponse = new MessageResponse();
		//object obj = messageResponse;
		//r = _serializer.TryDeserialize(fsdata, obj.GetType(), ref obj);
		//if (!r.Succeeded)
		//    throw new WatsonException(r.FormattedMessages);

		////  Set context for next round of messaging
		//object _tempContext = null;
		//Log.Debug("ConversationHandler.OnMessage()", "{0}", r);
		//(resp as Dictionary<string, object>).TryGetValue("context", out _tempContext);

		//if (_tempContext != null)
		//    _context = _tempContext as Dictionary<string, object>;
		//else
		//Log.Debug("ConversationHandler.OnMessage()", "Failed to get context");
	}

	private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
	{
		Log.Error("ConversationHandler.OnFail()", "Error received: {0}", error.ToString());
	}


}

