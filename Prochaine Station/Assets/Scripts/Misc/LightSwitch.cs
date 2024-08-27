using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static RoomLightManager;

public class LightSwitch : MonoBehaviour {
    public List<GameObject> spotLights; // List of spotlights to control
    public TMP_Text interactionText; // Reference to the Text element for interaction

    private bool lightsOn;
    private bool interactionEnabled = false; // Whether the interaction is enabled
    private Camera playerCamera;
    private AudioSource audioSource;

    [SerializeField]
    public AudioClip lightFlickSFX;

    private static bool anyInteractionEnabled = false; // Static variable to track any interaction enabled

    private void Start() {
        playerCamera = Camera.main;
        interactionText.enabled = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        playerCamera = Camera.main;
        interactionText.enabled = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void StartRaycast() {
        StartCoroutine(CheckForRaycast());
    }

    private IEnumerator CheckForRaycast() {
        while (true) {
            lightsOn = FindObjectOfType<RoomLightManager>().GetRoomByName(gameObject.name.Split("_")[0]).isActive;

            yield return new WaitForSeconds(0.1f);

            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool currentInteractionEnabled = false;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider != null && hit.collider.gameObject == gameObject) {
                    interactionText.text = "TURN LIGHTS " + (lightsOn ? "OFF " : "ON ") + "(E)";
                    currentInteractionEnabled = true;
                }
            }

            if (currentInteractionEnabled) {
                interactionText.enabled = true;
                anyInteractionEnabled = true;
                interactionEnabled = true;
            } else {
                interactionEnabled = false;
            }

            anyInteractionEnabled = false;
            foreach (var lightSwitch in FindObjectsOfType<LightSwitch>()) {
                if (lightSwitch.interactionEnabled) {
                    anyInteractionEnabled = true;
                    break;
                }
            }

            if (!interactionEnabled && !anyInteractionEnabled) {
                interactionText.enabled = false;
            }
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E) && interactionEnabled) {
            ToggleLights();
            audioSource.PlayOneShot(lightFlickSFX);
        }
    }

    private void ToggleLights() {
        lightsOn = !lightsOn;
        Room currentRoom = FindObjectOfType<RoomLightManager>().GetRoomByName(gameObject.name.Split("_")[0]);
        currentRoom.isActive = lightsOn;
        SetLightsState(currentRoom.lights, lightsOn);
    }
}
