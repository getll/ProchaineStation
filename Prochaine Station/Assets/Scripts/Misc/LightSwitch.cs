using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public List<GameObject> spotLights; // List of spotlights to control
    public TMP_Text interactionText; // Reference to the Text element for interaction
    public float rotationAngle = 180f; // Rotation angle for the light switch

    private bool lightsOn;
    private bool interactionEnabled = false; // Whether the interaction is enabled
    private Camera playerCamera;
    private AudioSource audioSource;

    [SerializeField]
    public AudioClip lightFlickSFX;

    private static bool anyInteractionEnabled = false; // Static variable to track any interaction enabled

    private void Start()
    {
        playerCamera = Camera.main;
        interactionText.enabled = false;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(CheckForRaycast()); // Start the coroutine for raycast checks
    }

    private IEnumerator CheckForRaycast()
    {
        while (true)
        {
            lightsOn = FindObjectOfType<RoomLightManager>().roomActiveStates[GetComponent<Transform>().parent.gameObject.name];

            yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds before the next check

            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool currentInteractionEnabled = false;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    interactionText.text = "TURN LIGHTS " + (lightsOn ? "OFF " : "ON ") + "(E)";
                    currentInteractionEnabled = true; // This instance wants to enable interaction
                }
            }

            if (currentInteractionEnabled)
            {
                interactionText.enabled = true; // Show the interaction text
                anyInteractionEnabled = true; // At least one instance wants to enable interaction
                interactionEnabled = true;
            }
            else
            {
                interactionEnabled = false; // This instance does not want to enable interaction
            }

            // Check if any instance is currently enabling interaction
            anyInteractionEnabled = false;
            foreach (var lightSwitch in FindObjectsOfType<LightSwitch>())
            {
                if (lightSwitch.interactionEnabled)
                {
                    anyInteractionEnabled = true;
                    break;
                }
            }

            if (!interactionEnabled && !anyInteractionEnabled)
            {
                interactionText.enabled = false; // Hide the interaction text only if no instances want to enable it
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && interactionEnabled)
        {
            ToggleLights();
            audioSource.PlayOneShot(lightFlickSFX);
        }
    }

    private void ToggleLights()
    {
        lightsOn = !lightsOn;
        FindObjectOfType<RoomLightManager>().roomActiveStates[GetComponent<Transform>().parent.gameObject.name] = lightsOn;
        foreach (var light in spotLights)
        {
            light.SetActive(lightsOn);

            Renderer renderer = light.GetComponent<Transform>().parent.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (lightsOn)
                {
                    // Enable emission
                    renderer.material.EnableKeyword("_EMISSION");
                }
                else
                {
                    // Disable emission
                    renderer.material.DisableKeyword("_EMISSION");
                }
            }
        }
    }
}
