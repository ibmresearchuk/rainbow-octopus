/**
* Copyright 2015 IBM Corp. All Rights Reserved.
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
*
*/

// This class has been adapted from the example tone analyzer, which is part of the
// Watson Unity SDK. Rather that running examples, it has a Get Tone function which
// can be called from the SpeechToText Handler, or other handlers if required.

using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.Collections;
using IBM.Watson.DeveloperCloud.Connection;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;

public class ToneHandler : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Tooltip("Text field to display the results of streaming.")]
    public Text ResultsField;
    [Tooltip("Text field to display the percentage of tone confidence.")]
    public Text PercentageField;
    [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/tone-analyzer/api\"")]
    [SerializeField]
    private string _serviceUrl;
    [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
    [SerializeField]
    private string _versionDate;
    [Header("CF Authentication")]
    [Tooltip("The authentication username.")]
    [SerializeField]
    private string _username;
    [Tooltip("The authentication password.")]
    [SerializeField]
    private string _password;
    [Header("IAM Authentication")]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string _iamApikey;
    [Tooltip("The IAM url used to authenticate the apikey (optional). This defaults to \"https://iam.bluemix.net/identity/token\".")]
    [SerializeField]
    private string _iamUrl;
    #endregion

    private ToneAnalyzer _service;
    private float emotion_threshold = 0.65f;
    private Text Tonea1, Tonea2, TonePercenta1, TonePercenta2;
    private Renderer r, rendEyeRight, rendEyeLeft;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        Tonea1 = GameObject.Find("Tonea1").GetComponent<Text>();                    // Previous two debug fields for Tone. This approach is OK when we only want 3 lines of debug appearing.
        Tonea2 = GameObject.Find("Tonea2").GetComponent<Text>();                    // Previous two debug fields for Tone. This approach is OK when we only want 3 lines of debug appearing.
        TonePercenta1 = GameObject.Find("TonePercenta1").GetComponent<Text>();      // Previous two debug fields for Tone Percentage. This approach is OK when we only want 3 lines of debug appearing.
        TonePercenta2 = GameObject.Find("TonePercenta2").GetComponent<Text>();      // Previous two debug fields for Tone Percentage. This approach is OK when we only want 3 lines of debug appearing.
                                                                                    // We have two fields (one for tone, one for confidence percentage) so that the Tone can be compared
                                                                                    // for the last 3 sentences, without comparing the percentage. There are of course other ways this could be done.
        Runnable.Run(CreateService());
    }

    // Directly from the Watson Unity SDK example, but removing running of the examples
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

        _service = new ToneAnalyzer(credentials);
        _service.VersionDate = _versionDate;

    //    Runnable.Run(ConversationHandlers());
    }

    public void GetTone(string question)
    {
        Log.Debug("ToneHandler.GetTone()", "{0}", question);

        if (!_service.GetToneAnalyze(OnGetToneAnalyze, OnFail, question))
            Log.Debug("ToneHandler.GetTone()", "Failed to analyze!");

    }

    private void OnGetToneAnalyze(ToneAnalyzerResponse resp, Dictionary<string, object> customData)
    {

        // Example response from Tone Analyzer ( Version  2017-05-25)
        //    { "document_tone":{ "tones":[{"score":1.0,"tone_id":"joy","tone_name":"Joy"}]}}



        double anger = resp.document_tone.tone_categories[0].tones[0].score;
        double disgust = resp.document_tone.tone_categories[0].tones[1].score;
        double fear = resp.document_tone.tone_categories[0].tones[2].score;
        double joy = resp.document_tone.tone_categories[0].tones[3].score;
        double sadness = resp.document_tone.tone_categories[0].tones[4].score;


        var tones = new SortedDictionary<string, double> {
    { "anger",  anger},
    { "disgust",  disgust},
    { "fear",  fear},
    { "joy",  joy},
    { "sadness",  sadness},
    };

        // max_tone gets the tone which has the highest value.
        string max_tone = tones.Aggregate((l, right) => l.Value > right.Value ? l : right).Key;

        //if this tone is bigger than the threshold we set, then we'll change the Octopus colour.
        if (tones[max_tone] > emotion_threshold)
        {
            Log.Debug("ToneHandler.OnGetToneAnalyze()", "Growing Max Tone = {0}", max_tone);
            //text_scroll.addline("test", max_tone);
  //          emotional_states[max_tone] += emotional_growth;


            Color color = Color.red;                                // Default color to be overwritten.
            Tonea2.text = Tonea1.text;                              // Move old tone values up the debug log in the Debug Panel.
            TonePercenta2.text = TonePercenta1.text;
            Tonea1.text = ResultsField.text;
            TonePercenta1.text = PercentageField.text;
            ResultsField.text = max_tone;                           // Set the current value in the Debug Panel
            PercentageField.text = "(" + tones[max_tone]*100 + "%)";
            int intensity = 3;                   // This is by default, a new emotion we've not seen before
            if (Tonea1.text.CompareTo(ResultsField.text)==0)
            {
                intensity = 2;                  // This is the same emotion twice
                if (Tonea2.text.CompareTo( ResultsField.text)==0) {
                    intensity = 1;              // This is the same emotion three times
                }

            }

            //  Set the color of the octopus depending on the tone from tone analyzer.
            //  Multiply some of the RGB values to change the color based on intensity value set above.

            switch (max_tone) {
                case "joy" : {
                        color = new Color(0.2f*intensity, 0.9f, 0.2f*intensity);
                        break;
                    }
                case "anger" : {
                        color = new Color(0.9f,0.2f*intensity,0.2f*intensity);
                        break;
                    }
                case "fear":
                    {
                        color = new Color(0.9f, 0.9f, 0.2f * intensity);
                        break;
                    }
                case "sadness":
                    {
                        color = new Color(0.2f * intensity,0.2f * intensity,0.2f *intensity);
                        break;
                    }
                case "disgust":
                    {
                        color = new Color(0.9f, 0.2f * intensity, 0.9f);
                        break;
                    }
            }

            // Here we set the color - the Octopus has 3 renderers, one for the body (with 2 materials), and one for each eye - we'll adjust the color of each to the
            // same color here, but of course these could be set to different colors if you wanted just parts of the Octopus to change.
            Log.Debug("ToneHandler.OnGetToneAnalyze()", "Setting color = {0}", color);
            r = GameObject.Find("body").GetComponent<Renderer>();
            rendEyeRight = GameObject.Find("eye - right").GetComponent<Renderer>();
            rendEyeLeft = GameObject.Find("eye - left").GetComponent<Renderer>();
            r.materials[0].SetColor("_Color", color);
            r.materials[1].SetColor("_Color", color);
            rendEyeLeft.material.SetColor("_Color", color);
            rendEyeRight.material.SetColor("_Color", color);

            Log.Debug("ToneHandler.OnGetToneAnalyze()", "Growing Max Tone = {0}", max_tone);
        }
        else        // If the tone measured has a lower confidence level, then we'll indicate that we're not confident
                    // and not change the color.
        {
            Log.Debug("ToneHandler.OnGetToneAnalyze()", "Max tone below Threshold {0}", emotion_threshold);
            Tonea2.text = Tonea1.text;
            TonePercenta2.text = TonePercenta1.text;
            Tonea1.text = ResultsField.text;
            TonePercenta1.text = PercentageField.text;
            PercentageField.text = "";
            ResultsField.text = "Tone not clear";
        }


    }

    // Directly from the Watson Unity SDK example.
    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleRetrieveAndRank.OnFail()", "Error received: {0}", error.ToString());
    }
}
