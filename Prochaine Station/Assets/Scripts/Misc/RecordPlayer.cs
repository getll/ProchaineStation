using System.Collections;
using TMPro;
using UnityEngine;

public class RecordPlayer : MonoBehaviour
{
    public TMP_Text interactionText; // Reference to the Text element for interaction
    public AudioClip recordClip; // Audio clip to play on the record player
    public AudioClip toggleSFX; // Audio clip to play on the record player

    private bool isPlaying;
    private bool interactionEnabled = false; // Whether the interaction is enabled
    private Camera playerCamera;
    private AudioSource audioSource;

    private static bool anyInteractionEnabled = false; // Static variable to track any interaction enabled

    private void Start()
    {
        playerCamera = Camera.main;
        interactionText.enabled = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = recordClip;
        StartCoroutine(CheckForRaycast()); // Start the coroutine for raycast checks
    }

    private IEnumerator CheckForRaycast()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds before the next check

            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool currentInteractionEnabled = false;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    interactionText.text = (isPlaying ? "TURN OFF " : "TURN ON ") + "(E)";
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
            foreach (var recordPlayer in FindObjectsOfType<RecordPlayer>())
            {
                if (recordPlayer.interactionEnabled)
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
            audioSource.PlayOneShot(toggleSFX);
            TogglePlayback();
        }
    }

    private void TogglePlayback()
    {
        if (isPlaying)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.Play();
        }
        isPlaying = !isPlaying;
    }
}
