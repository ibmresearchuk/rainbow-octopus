using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARAmbientLight : MonoBehaviour
{
    public bool matchColorTemperature = true;

    [Range(0, 1)]
    public float colorCorrectionStrength = 0.5f;

    public bool matchBrightness = true;

    [Range(0, 1)]
    public float brightnessMatchingStrength = 0.5f;

    public float changeSmoothing = 0.2f;

    private float initialAmbientBrightness;

    private Color ambientColor;

    void Awake()
    {
        ARSubsystemManager.cameraFrameReceived += FrameChanged;
    }

    void Start()
    {
        ambientColor = RenderSettings.ambientLight;
        initialAmbientBrightness = ambientColor.maxColorComponent;
    }

    void Update()
    {
        if (changeSmoothing > 0)
        {
            float ratio = Time.deltaTime / changeSmoothing;

            RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, ambientColor, ratio);
        }
        else
        {
            RenderSettings.ambientLight = ambientColor;
        }
    }

    void FrameChanged(ARCameraFrameEventArgs args)
    {
        float brightness = initialAmbientBrightness;
        Color colorTempRGB = Color.white;

        // WARNING: colorCorrection only exists on Android.
        if (matchColorTemperature && args.lightEstimation.colorCorrection.HasValue)
        {
            Debug.LogWarning("Color correction for ARCore has not been implemented, yet.");
        }

        if (matchBrightness && args.lightEstimation.averageBrightness.HasValue)
        {
            float deltaBrightness = (args.lightEstimation.averageBrightness.Value - 0.5f) * brightnessMatchingStrength;
			brightness = Mathf.Clamp(initialAmbientBrightness + deltaBrightness, 0, 1);
        }

        // WARNING: averageColorTemperature only exists on iOS.
        if (matchColorTemperature && args.lightEstimation.averageColorTemperature.HasValue)
        {
            float colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            colorTempRGB = Mathf.CorrelatedColorTemperatureToRGB(colorTemperature);
        }


        Color colorOffset = Color.white * (1f - colorCorrectionStrength);
        ambientColor = (colorTempRGB * colorCorrectionStrength + colorOffset) * brightness;
    }
}


