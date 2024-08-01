using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SkyboxChanger : MonoBehaviour {
    [SerializeField] public string timeOfDay;
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;
    [SerializeField] private KeyCode toggleKey = KeyCode.T;
    [SerializeField] private KeyCode toggleInteriorKey = KeyCode.I;
    [SerializeField] private GameObject[] interiorObjects; // Ensure there are exactly 2 objects in this array
    public AudioSource ambientNoise;
    public AudioClip dayNoise;
    public AudioClip nightNoise;
    public TMP_Text stateInfoText; // Reference to the UI Text element
    public string interiorState;

    private int currentInteriorIndex = 0;

    void Start() {
        UpdateSkybox();
        UpdateInterior();
        UpdateStateInfoText();
    }

    void Update() {
        if (Input.GetKeyDown(toggleKey)) {
            ToggleTimeOfDay();
            UpdateStateInfoText();
        }

        if (Input.GetKeyDown(toggleInteriorKey)) {
            ToggleInterior();
            UpdateStateInfoText();
        }
    }

    void ToggleTimeOfDay() {
        timeOfDay = (timeOfDay == "Day") ? "Night" : "Day";
        UpdateSkybox();
    }

    void UpdateSkybox() {
        if (timeOfDay == "Day") {
            RenderSettings.skybox = daySkybox;
            RenderSettings.ambientIntensity = 1f;
            RenderSettings.fogDensity = 0.01f;
        } else if (timeOfDay == "Night") {
            RenderSettings.skybox = nightSkybox;
            RenderSettings.ambientIntensity = 1.5f;
            RenderSettings.fogDensity = 0.3f;
        } else {
            Debug.LogWarning("Invalid timeOfDay value. Please set it to 'Day' or 'Night'.");
        }

        PlaySound(timeOfDay);
        DynamicGI.UpdateEnvironment(); // Update the lighting environment
    }

    void PlaySound(string timeOfDay) {
        ambientNoise.clip = timeOfDay == "Day" ? dayNoise : nightNoise;
        ambientNoise.loop = true; // Set to loop
        ambientNoise.Play(); // Play the sound
    }

    public void SetTimeOfDay(string newTimeOfDay) {
        timeOfDay = newTimeOfDay;
        UpdateSkybox();
    }

    void ToggleInterior() {
        currentInteriorIndex = (currentInteriorIndex + 1) % interiorObjects.Length;
        UpdateInterior();
    }

    void UpdateInterior() {
        FindObjectOfType<PlayerControllerScript>().BroadcastMessage("SwitchModels");

        for (int i = 0; i < interiorObjects.Length; i++) {
            // Check if the current interior object is active
            bool isActive = i == currentInteriorIndex;
            interiorObjects[i].SetActive(isActive);

            // If the interior object is being disabled, find and disable the RecordPlayer script
            if (!isActive) {
                RecordPlayer recordPlayer = interiorObjects[i].GetComponentInChildren<RecordPlayer>();
                if (recordPlayer != null) {
                    recordPlayer.enabled = false; // Disable the RecordPlayer script
                }
            }
            // If the interior object is being enabled, find and enable the RecordPlayer script if needed
            else {
                RecordPlayer recordPlayer = interiorObjects[i].GetComponentInChildren<RecordPlayer>();
                if (recordPlayer != null) {
                    recordPlayer.enabled = true; // Enable the RecordPlayer script
                }
            }
        }
    }


    void UpdateStateInfoText() {
        if (stateInfoText != null) {
            interiorState = currentInteriorIndex == 0 ? "Proper" : "Run-down";
            stateInfoText.text = $"Press '{toggleKey}' to toggle Day/Night. Current: {timeOfDay}\n" +
                                 $"Press '{toggleInteriorKey}' to toggle Interior State. Current: {interiorState}";
        } else {
            Debug.LogWarning("StateInfoText is not assigned in the inspector.");
        }
    }
}
