# RainbowOctopus

In this Code Pattern, we will build an Octopus in Unity, which we deploy into an AR environment on an iPhone/iPad using ARKit. We can talk to the Octopus using IBM Speech To Text, Tone Analyzer, and Watson Assistant, and it will respond in various ways. 


When the reader has completed this Code Pattern, they will understand how to:

* Integrate the Watson Unity SDK into a project
* Integrate the ARKit SDK into a project.
* Capture and send speech to the IBM Speech To Text, and interpret the resultsTone Analyzer and Watson Assistant.

* Implement a chatbot with Watson Assistant and embed it on a web page with Node-RED.
* Get satellite information for the International Space Station (ISS) and use it in a web app.


## Flow

1. User starts the app on the iPad.
2. ARKit identifies flat planes within the camera view, and places a beach on a flat surface when it finds one.
3. The user touches the beach - and the octopus will appear and can be moved round the beach.
4. The app listens for speech. The user talks to the Octopus, saying things such as :
* "Hello! " (Octopus will wave)
* "Go for a walk" (Octopus will start walking)
* "Stop!" (Octopus will get scared - changing colour to yellow - and stop in his tracks)
* "Jump for Joy" (Octopus will jump up and down, whilst glowing green)
* "Get bigger / smaller" (Octopus will grow / shrink)
* A variety of other emotive sentences that cause the Octopus to change colour. Reinforcing that emotion will make that colour become stronger.
5. The app sends the speech to Watson Speech-To-Text (via the Watson Unity SDK), and receives raw text in response.
6. The app passes the raw text to Watson Assistant, and simultaneously to Watson Tone Analyzer - again utilizing the Watson Unity SDK.
7. The results from the Tone Analyzer determine what tone was identified from the stated speech - this may be "joy", "sadness", "anger" etc. - and colours the octopus accordingly. 
8. The intent from the results of the Watson Assistant are also analyzed, and may cause the octopus to walk, jump, get bigger or smaller.

## Included components

* [Watson Assistant](https://www.ibm.com/watson/services/conversation/): Create a chatbot with a program that conducts a conversation via auditory or textual methods.
* [Watson Speech-to-Text](https://www.ibm.com/watson/services/speech-to-text/):Easily convert audio and voice into written text for quick understanding of content. 
* [Watson Tone Analyzer](https://www.ibm.com/watson/services/tone-analyzer/):Understand emotions and communication style in text. 
* [Watson Unity SDK](https://github.com/watson-developer-cloud/unity-sdk): Use this SDK to build Watson-powered applications in Unity.
* [Unity ARKit Plugin](https://assetstore.unity.com/packages/essentials/tutorial-projects/unity-arkit-plugin-92515): The experimental plugin exposes ARKit SDK's world tracking capabilities, rendering the camera video input, plane detection and update, point cloud extraction, light estimation, and hit testing API to Unity developers for their AR projects.


<!--
# Watch the Video
-->

# Steps

## Run locally

1. [Clone the repo](#1-clone-the-repo)
1. [Create Watson services with IBM Cloud](#2-create-watson-services-with-ibm-cloud)
1. [Import the Watson Assistant workspace](#3-import-the-watson-assistant-workspace)
1. [Add libraries to Unity Project](#4-get-the-watson-assistant-credentials)
1. [Add credentials to Unity](#5-add-credentials-to-unity)
1. [Build and run](#6-build-and-run)

### 1. Clone the repo

Clone the `RainbowOctopus` repo locally. In a terminal, run:

```
$ git clone https://github.com/kevinxbrown/RainbowOctopus.git
```

### 2. Create Watson services with IBM Cloud

Create the [*Watson Assistant*](https://console.ng.bluemix.net/catalog/services/conversation) service by providing a name of your choice and clicking `Create`.

Once created, you'll see either the credentials for *username* and *password* or an IAM *apikey*, either of which you should copy down to be used later. (Click `Show` to expose them).

Repeat for [*Watson Tone Analyzer](https://console.bluemix.net/catalog/services/tone-analyzer), taking note of the *username* and *password*.

Repeat for [*Watson Speech To Text](https://console.bluemix.net/catalog/services/speech-to-text), again taking note of the *username* and *password*.

![](https://github.com/IBM/pattern-images/blob/master/watson-assistant/WatsonAssistantCredentials.png)

![](https://github.com/IBM/pattern-images/blob/master/watson-assistant/watson_assistant_api_key.png)

### 3. Import the Watson Assistant workspace

Once you have created your instance of Watson Assistant, click `Launch Tool`, then click the `Workspaces` tab. Import the workspace by clicking the upload icon:

![](https://github.com/IBM/pattern-images/blob/master/watson-assistant/UploadWorkspaceJson.png)

Click `Choose a file` and navigate to [`/RainbowOctopusWatsonAssistantSample.json`](/RainbowOctopusWatsonAssistantSample.json) in this repo. Click `Import`.

Get the Workspace ID by clicking the 3 vertical dots on the `Workspaces` tab. Save this for later.

<p align="center">
  <img width="200" height="300" src="https://github.com/IBM/pattern-images/blob/master/watson-assistant/GetAssistantDetails.png">
</p>


### 4. Add libraries to Unity project

Download and unzip the [Watson Unity SDK](https://github.com/watson-developer-cloud/unity-sdk). The project was created using version 2.4.0.

In Unity, in the Project Explorer, click on Assets. Drag the unziped unity-sdk-x.x.x folder (on the file system) into the Assets window (in Unity).

Download and unzip the [Unity ARKit Plugin Repo](https://bitbucket.org/Unity-Technologies/unity-arkit-plugin/overview). Within the Assets folder, there is a UnityARKitPlugin folder. Drag this folder (on the file system) into the Assets window (in Unity) alongside the unity-sdk-2.4.0 just created.

Your Assets folder in Unity should now contain 3 folders - RainbowOctopus, unity-sdk-x.x.x and UnityARKitPlugin.

### 5. Add credentials to Unity

The credentials for IBM Cloud Watson Assistant, Speech To Text and Tone Analyzer services can be found
by selecting the ``Service Credentials`` option for each service in Watson. You
saved these in [step #2](#2-create-watson-services-with-ibm-cloud).

The `WORKSPACE_ID` for the Watson Assistant workspace was saved in
[step #3](#3-import-the-watson-assistant-workspace).

In the Unity Hierarchy, choose `OctopusLogic`. In the Inspector, you will see all the scripts that handle the services. Place the Username and Password for each service in the relevant script's settings panel. In the Conversation Handler (Script) section, you'll also see the Workspace Id field - which needs updating with your Workspace Id. Finally, keep the version date configured as 2017-05-26. This is the version of the API we are using, which returns data from the services in the format the code is expecting.

### 6. Build and run
In Unity, choose `File` then `Build Settings...`. In the dialog box, ensure `RainbowOctopus/Scenes/RainbowOctopus` is selected. Ensure iOS is selected, and choose Build and Run. This will create an XCode project for you to deploy to your iOS device, and open the project in XCode.


# Sample output

![](doc/source/images/sample_output.png)

# Troubleshooting

# Links



# Learn more

* **Artificial Intelligence Code Patterns**: Enjoyed this Code Pattern? Check out our other [AI Code Patterns](https://developer.ibm.com/code/technologies/artificial-intelligence/).
* **AI and Data Code Pattern Playlist**: Bookmark our [playlist](https://www.youtube.com/playlist?list=PLzUbsvIyrNfknNewObx5N7uGZ5FKH0Fde) with all of our Code Pattern videos
* **With Watson**: Want to take your Watson app to the next level? Looking to utilize Watson Brand assets? [Join the With Watson program](https://www.ibm.com/watson/with-watson/) to leverage exclusive brand, marketing, and tech resources to amplify and accelerate your Watson embedded commercial solution.

# License
[Apache 2.0](LICENSE)
