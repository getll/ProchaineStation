using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    [SerializeField] private string timeOfDay;
    [SerializeField] private Material daySkybox; // Changed to Cubemap
    [SerializeField] private Material nightSkybox; // Changed to Cubemap
    [SerializeField] private KeyCode toggleKey = KeyCode.T;
    public AudioSource ambientNoise;
    public AudioClip dayNoise;
    public AudioClip nightNoise;

    void Start()
    {
        UpdateSkybox();
    }

    void Update()
    {
        // Check for key press to toggle day/night
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleTimeOfDay();
        }
    }

    void ToggleTimeOfDay()
    {
        // Toggle between "Day" and "Night"
        timeOfDay = (timeOfDay == "Day") ? "Night" : "Day";
        UpdateSkybox();
    }

    void UpdateSkybox()
    {
        if (timeOfDay == "Day")
        {
            RenderSettings.skybox = daySkybox;
            RenderSettings.ambientIntensity = 1f;
            RenderSettings.fogDensity = 0.01f;
        }
        else if (timeOfDay == "Night")
        {
            RenderSettings.skybox = nightSkybox;
            RenderSettings.ambientIntensity = 1.5f;
            RenderSettings.fogDensity = 0.1f;
        }
        else
        {
            Debug.LogWarning("Invalid timeOfDay value. Please set it to 'Day' or 'Night'.");
        }

        PlaySound(timeOfDay);
        DynamicGI.UpdateEnvironment(); // Update the lighting environment
    }

    void PlaySound(string timeOfDay)
    {
        ambientNoise.clip = timeOfDay == "Day" ? dayNoise : nightNoise;
        ambientNoise.loop = true; // Set to loop
        ambientNoise.Play(); // Play the sound
    }

    public void SetTimeOfDay(string newTimeOfDay)
    {
        timeOfDay = newTimeOfDay;
        UpdateSkybox();
    }
}
