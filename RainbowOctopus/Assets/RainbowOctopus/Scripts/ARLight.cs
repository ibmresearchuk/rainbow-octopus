using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Light))]
public class ARLight : MonoBehaviour
{
    Light m_Light;

    public bool matchColorTemperature = true;

    [Range(0, 1)]
    public float colorCorrectionStrength = 0.5f;

    public bool matchBrightness = true;

    [Range(0, 1)]
    public float brightnessMatchingStrength = 0.5f;

    public float changeSmoothing = 0.2f;

    private float initialIntensity;

	private float intensity;

	private Color initialColor;

	private Color ambientColor;

    void Awake()
    {
        m_Light = GetComponent<Light>();
        ARSubsystemManager.cameraFrameReceived += FrameChanged;
    }

	void Start()
    {
        initialIntensity = m_Light.intensity;
		intensity = initialIntensity;
		initialColor = m_Light.color;
		ambientColor = initialColor;
    }

	void Update()
    {
        if (changeSmoothing > 0)
        {
            float ratio = Time.deltaTime / changeSmoothing;
			m_Light.intensity = Mathf.Lerp(m_Light.intensity, intensity, ratio);
			m_Light.color = Color.Lerp(m_Light.color, ambientColor, ratio);
        }
        else
        {
            m_Light.intensity = intensity;
			m_Light.color = ambientColor;
        }
    }

    void FrameChanged(ARCameraFrameEventArgs args)
    {
        // WARNING: colorCorrection only exists on Android.
        if (matchColorTemperature && args.lightEstimation.colorCorrection.HasValue)
        {
            Debug.LogWarning("Color correction for ARCore has not been implemented, yet.");
        }

        if (matchBrightness && args.lightEstimation.averageBrightness.HasValue)
        {
            float deltaBrightness = (args.lightEstimation.averageBrightness.Value - 0.5f) * 2f * brightnessMatchingStrength;
            intensity = Mathf.Clamp(initialIntensity + deltaBrightness, 0, 1);
        }

        // WARNING: averageColorTemperature only exists on iOS.
        if (matchColorTemperature && args.lightEstimation.averageColorTemperature.HasValue)
        {
            float colorTemperature = args.lightEstimation.averageColorTemperature.Value;
            Color colorTempRGB = Mathf.CorrelatedColorTemperatureToRGB(colorTemperature);
			ambientColor = Color.Lerp(initialColor, colorTempRGB, colorCorrectionStrength);
		}
    }

}









