using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class SkyboxChanger : MonoBehaviour {
    [SerializeField] public string timeOfDay;
    public Color dayColor = new Color32(123, 152, 193, 255); // RGB for 7B98C1
    [SerializeField] private Color nightColor;
    [SerializeField] private KeyCode toggleKey = KeyCode.T;
    [SerializeField] private KeyCode toggleInteriorKey = KeyCode.I;
    [SerializeField] private GameObject[] interiorObjects; // Ensure there are exactly 2 objects in this array
    public AudioSource ambientNoise;

    public float dayIntensity = 3f;
    public float nightIntensity = 0.5f;

    public AudioClip dayNoise;
    public AudioClip nightNoise;
    public TMP_Text stateInfoText; // Reference to the UI Text element
    public string interiorState;
    [SerializeField]
    private float dayFogEnd = 300f;
    [SerializeField]
    private float nightFogEnd = 40f; // Slightly smaller than day

    [SerializeField]
    List<Light> outdoorLights;

    private int currentInteriorIndex = 0;

    void Start() {
        // Load timeOfDay and interior state from PlayerPrefs
        timeOfDay = PlayerPrefs.GetString("TimeOfDay", "Day");
        currentInteriorIndex = PlayerPrefs.GetInt("InteriorState", 0);

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        UpdateSkybox();
        UpdateInterior();
        UpdateStateInfoText();
    }

    void Update() {
        if (Input.GetKeyDown(toggleKey)) {
            ToggleTimeOfDay();
            UpdateStateInfoText();
            FindObjectOfType<RoomLightManager>().BroadcastMessage("UpdateRoomLighting");
        }

        if (Input.GetKeyDown(toggleInteriorKey)) {
            ToggleInterior();
            UpdateStateInfoText();
            FindObjectOfType<RoomLightManager>().BroadcastMessage("UpdateRoomLighting");
        }

        RotateSkybox();
    }

    void ToggleTimeOfDay() {
        timeOfDay = (timeOfDay == "Day") ? "Night" : "Day";
        UpdateSkybox();

        // Save the timeOfDay to PlayerPrefs
        PlayerPrefs.SetString("TimeOfDay", timeOfDay);
        PlayerPrefs.Save();
    }

    void UpdateSkybox() {
        foreach (Light light in outdoorLights) {
            light.enabled = timeOfDay == "Night";
            Transform lightShape = light.transform.Find("LightShape");
            if (lightShape != null) { lightShape.GetComponent<Renderer>().enabled = timeOfDay == "Night"; }
        }

        if (timeOfDay == "Day") {
            Camera.main.backgroundColor = dayColor;
            RenderSettings.fogColor = dayColor;
            RenderSettings.fogStartDistance = 0f;
            RenderSettings.ambientIntensity = dayIntensity;
            RenderSettings.fogEndDistance = dayFogEnd;
        } else if (timeOfDay == "Night") {
            Camera.main.backgroundColor = nightColor;
            RenderSettings.fogColor = nightColor;
            RenderSettings.fogStartDistance = 0f;
            RenderSettings.ambientIntensity = nightIntensity;
            RenderSettings.fogEndDistance = nightFogEnd;
        } else {
            Debug.LogWarning("Invalid timeOfDay value. Please set it to 'Day' or 'Night'.");
        }

        PlaySound(timeOfDay);
    }

    void PlaySound(string timeOfDay) {
        ambientNoise.clip = timeOfDay == "Day" ? dayNoise : nightNoise;
        ambientNoise.loop = true; // Set to loop
        ambientNoise.Play(); // Play the sound
    }

    public void SetTimeOfDay(string newTimeOfDay) {
        timeOfDay = newTimeOfDay;
        UpdateSkybox();

        // Save the timeOfDay to PlayerPrefs
        PlayerPrefs.SetString("TimeOfDay", timeOfDay);
        PlayerPrefs.Save();
    }

    void ToggleInterior() {
        currentInteriorIndex = (currentInteriorIndex + 1) % interiorObjects.Length;
        UpdateInterior();

        // Save the interior state to PlayerPrefs
        PlayerPrefs.SetInt("InteriorState", currentInteriorIndex);
        PlayerPrefs.Save();
    }

    void UpdateInterior() {
        FindObjectOfType<PlayerControllerScript>().BroadcastMessage("SwitchModels");

        // Get the current room name from the RoomLightManager
        string currentRoomName = FindObjectOfType<RoomLightManager>().currentRoom.roomName;

        for (int i = 0; i < interiorObjects.Length; i++) {
            // Check if the current interior object is active
            bool isActive = i == currentInteriorIndex;
            interiorObjects[i].SetActive(isActive);

            // Get the RecordPlayer component in the children of the current interior object
            RecordPlayer recordPlayer = interiorObjects[i].GetComponentInChildren<RecordPlayer>();

            // Check if the RecordPlayer exists and if its parent has the same name as the current room
            if (recordPlayer != null && recordPlayer.transform.parent.name == currentRoomName) {
                recordPlayer.enabled = isActive;
            }
        }
    }

    void UpdateStateInfoText() {
        if (stateInfoText != null) {
            interiorState = currentInteriorIndex == 0 ? "PROPER" : "RUN-DOWN";
            stateInfoText.text = $"'{toggleKey}' TOGGLES TIME. CURRENT: {timeOfDay.ToUpper()}\n" +
                                 $"'{toggleInteriorKey}' TOGGLES INTERIOR. CURRENT: {interiorState}";
        } else {
            Debug.LogWarning("StateInfoText is not assigned in the inspector.");
        }
    }

    void RotateSkybox() {
        // This function can be removed or modified if the rotation is not needed
    }
}
