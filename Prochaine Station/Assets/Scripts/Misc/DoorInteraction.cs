using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorInteraction : MonoBehaviour
{
    public TMP_Text interactionText; // Reference to the Text element
    public Image screenTransition; // Reference to the Panel element
    public float transitionTime = 2f; // Time for the screen transition

    private PlayerControllerScript characterController;
    private Transform currentDoor; // Reference to the current door
    private Transform targetPosition; // Target position to move the player to
    private RoomLightManager roomLightManager;
    private bool enumerator;

    private Camera playerCamera;
    private Collider currentDoorCollider;
    private float checkDelay = 0.1f;
    private float lastCheckTime = 0f;

    [SerializeField]
    public AudioSource audioSource;

    [SerializeField]
    public AudioClip doorOpenSFX;
    [SerializeField]
    public AudioClip doorCloseSFX;

    private void Start()
    {
        characterController = GetComponent<PlayerControllerScript>();
        roomLightManager = FindObjectOfType<RoomLightManager>();
        interactionText.enabled = false;
        var color = screenTransition.color;
        color.a = 0f;
        screenTransition.color = color;

        playerCamera = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            interactionText.text = "OPEN (E)";
            interactionText.enabled = true;
            currentDoor = other.GetComponentInParent<Transform>().parent;
            currentDoorCollider = currentDoor.GetComponentInParent<Collider>(); // Store the collider of the door for raycast checks
            if (other.name == "Out")
            {
                targetPosition = currentDoor.Find("In");
            }
            else if (other.name == "In")
            {
                targetPosition = currentDoor.Find("Out");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            interactionText.enabled = false; // Hide the interaction text
            currentDoor = null; // Clear the reference to the door
            currentDoorCollider = null; // Clear the reference to the door collider
            targetPosition = null;
        }
    }

    private void Update()
    {
        // Check if enough time has passed since the last check
        if (Time.time - lastCheckTime >= checkDelay)
        {
            lastCheckTime = Time.time;

            if (currentDoor != null && currentDoorCollider != null)
            {
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == currentDoorCollider && !enumerator)
                    {
                        interactionText.enabled = true; // Show the interaction text
                    }
                    else
                    {
                        interactionText.enabled = false; // Hide the interaction text
                    }
                }
                else
                {
                    interactionText.enabled = false; // Hide the interaction text
                }
            }
            else
            {
                interactionText.enabled = false; // Hide the interaction text
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && targetPosition != null && !enumerator && interactionText.enabled)
        {
            StartCoroutine(InteractWithDoor());
            enumerator = true; // Set flag to prevent multiple interactions
        }
    }

    private IEnumerator InteractWithDoor()
    {
        interactionText.enabled = false;
        audioSource.PlayOneShot(doorOpenSFX);
        float originalSpeed = characterController.speed;
        characterController.speed = 0f;

        // Start the screen transition to black
        yield return StartCoroutine(FadeToBlack());

        audioSource.PlayOneShot(doorCloseSFX);

        GetComponent<Collider>().enabled = false;

        if (targetPosition != null)
        {
            Vector3 targetWorldPosition = currentDoor.TransformPoint(targetPosition.localPosition);
            transform.position = targetWorldPosition;
        }
        else
        {
            Debug.LogWarning("Target position is null. Cannot move player.");
        }

        GetComponent<Collider>().enabled = true;

        yield return new WaitForSeconds(0.5f);

        String activeRoom = currentDoor.gameObject.name.Split("_")[1];

        if (activeRoom == roomLightManager.currentActiveRoom)
        {
            roomLightManager.SetActiveRoom("Kitchen");
        }
        else
        {
            roomLightManager.SetActiveRoom(activeRoom);
        }

        yield return StartCoroutine(FadeToClear());

        characterController.speed = originalSpeed;

        yield return new WaitForSeconds(0.1f);

        enumerator = false;
    }

    private IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        Color color = screenTransition.color;

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / transitionTime);
            screenTransition.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeToClear()
    {
        float elapsedTime = 0f;
        Color color = screenTransition.color;

        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsedTime / transitionTime);
            screenTransition.color = color;
            yield return null;
        }
    }
}
