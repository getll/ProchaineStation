using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static RoomLightManager;

public class DoorInteraction : MonoBehaviour {
    public TMP_Text interactionText; // Reference to the Text element
    public Image screenTransition; // Reference to the Panel element
    public float transitionTime = 1f; // Time for the screen transition
    public bool outsideLevel;

    private PlayerControllerScript characterController;
    private Transform currentDoor; // Reference to the current door
    private Transform targetPosition; // Target position to move the player to
    private RoomLightManager roomLightManager;
    private bool enumerator;

    private Camera playerCamera;
    private Collider currentDoorCollider;
    private List<Collider> disabledColliders = new List<Collider>();
    private float checkDelay = 0.1f;
    private float lastCheckTime = 0f;

    [SerializeField]
    public AudioSource audioSource;

    [SerializeField]
    public AudioClip doorOpenSFX;
    [SerializeField]
    public AudioClip doorCloseSFX;

    private void Start() {
        characterController = GetComponent<PlayerControllerScript>();
        roomLightManager = FindObjectOfType<RoomLightManager>();

        interactionText.enabled = false;

        StartCoroutine(FadeToClear());

        playerCamera = Camera.main;
    }

    private void OnEnable() {
        string originatingScene = PlayerPrefs.GetString("OriginatingScene", "");
        if (!string.IsNullOrEmpty(originatingScene)) {
            string doorObjectName = $"{originatingScene}_Door";
            GameObject doorObject = GameObject.Find(doorObjectName);
            if (doorObject != null) {
                Transform inPosition = doorObject.transform.Find("Spawn");
                if (inPosition != null) {
                    GetComponent<Collider>().enabled = false;
                    transform.SetPositionAndRotation(inPosition.position, inPosition.rotation);
                    GetComponent<Collider>().enabled = true;
                    FindObjectOfType<RoomLightManager>().SetActiveRoom(doorObject.transform.parent.name + "Outside");
                } else {
                    Debug.LogWarning("In position not found for door: " + doorObjectName);
                }
            } else {
                Debug.LogWarning("Door object not found: " + doorObjectName);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Door") || other.CompareTag("SceneDoor")) {
            currentDoor = other.GetComponentInParent<Transform>().parent;

            // Get all colliders in the parent object
            Collider[] colliders = currentDoor.GetComponents<Collider>();

            // Find the closest collider to the "other" collider
            currentDoorCollider = GetClosestCollider(other, colliders);

            // Disable the other colliders
            DisableOtherColliders(colliders, currentDoorCollider);

            // Determine the target position based on the collider's name
            if (other.name == "Out") {
                targetPosition = currentDoor.transform.Find("In");
            } else if (other.name == "In") {
                targetPosition = currentDoor.transform.Find("Out");
            }
        }
    }

    private Collider GetClosestCollider(Collider other, Collider[] colliders) {
        Collider closestCollider = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders) {
            Vector3 colliderCenter;

            // Check if the collider is a BoxCollider
            if (collider is BoxCollider boxCollider) {
                colliderCenter = boxCollider.bounds.center;
            } else {
                // Fallback to the transform's position if it's not a BoxCollider
                colliderCenter = collider.transform.position;
            }

            float distance = Vector3.Distance(other.bounds.center, colliderCenter);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestCollider = collider;
            }
        }

        return closestCollider;
    }


    private void DisableOtherColliders(Collider[] colliders, Collider closestCollider) {
        foreach (var collider in colliders) {
            if (collider != closestCollider) {
                collider.enabled = false;
                disabledColliders.Add(collider); // Add to the list of disabled colliders
            }
        }
    }


    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Door") || other.CompareTag("SceneDoor")) {
            interactionText.enabled = false;
            currentDoor = null;
            currentDoorCollider = null;
            targetPosition = null;
            foreach (var collider in disabledColliders) {
                collider.enabled = true;
            }

            // Clear the list of disabled colliders
            disabledColliders.Clear();
        }
    }

    private void Update() {
        StartCoroutine(RaycastDoor());

        if (Input.GetKeyDown(KeyCode.E) && targetPosition != null && !enumerator && interactionText.enabled) {
            interactionText.enabled = false;

            enumerator = true;

            string directingName = currentDoor.gameObject.name.Split('_')[0];

            if (currentDoor.CompareTag("SceneDoor")) {
                StartCoroutine(LoadScene(directingName));
            } else {
                StartCoroutine(InteractWithDoor());

                Debug.Log(directingName + " and " + roomLightManager.currentRoom.roomName);

                if (directingName == roomLightManager.currentRoom.roomName) {
                    roomLightManager.SetActiveRoom(roomLightManager.defaultRoom.roomName);
                } else {
                    roomLightManager.SetActiveRoom(directingName);
                }
            }
        }
    }

    private IEnumerator RaycastDoor() {
        // Check if enough time has passed since the last check
        if (Time.time - lastCheckTime >= checkDelay) {
            lastCheckTime = Time.time;

            if (currentDoor != null && currentDoorCollider != null) {
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    if (hit.collider == currentDoorCollider && !enumerator) {
                        interactionText.enabled = true; // Show the interaction text

                        string doorName = currentDoorCollider.transform.name.Split("_")[0].ToUpper();

                        if (doorName.ContainsInsensitive("Home")) {
                            interactionText.text = "HEAD HOME (E)";
                        } else if (doorName.ContainsInsensitive("Metro")) {
                            interactionText.text = "HEAD TO METRO (E)";
                        } else if (doorName.ContainsInsensitive("Store")) {
                            interactionText.text = "HEAD TO STORE (E)";
                        } else {
                            interactionText.text = "OPEN (E)";
                        }
                    } else {
                        interactionText.enabled = false; // Hide the interaction text
                    }
                } else {
                    interactionText.enabled = false; // Hide the interaction text
                }

            } else {
                interactionText.enabled = false; // Hide the interaction text
            }
        }

        yield return new WaitForSeconds(checkDelay);
    }

    private IEnumerator InteractWithDoor() {
        if (!outsideLevel) audioSource.PlayOneShot(doorOpenSFX);

        float originalSpeed = characterController.speed;

        characterController.speed = 0f;

        // Start the screen transition to black
        yield return StartCoroutine(FadeToBlack());

        if (!outsideLevel) audioSource.PlayOneShot(doorCloseSFX);

        GetComponent<Collider>().enabled = false;

        if (targetPosition != null) {
            // Set both position and rotation at the same time using SetPositionAndRotation
            transform.SetPositionAndRotation(targetPosition.position, targetPosition.rotation);
        } else {
            Debug.LogWarning("Target position is null. Cannot move player.");
        }

        GetComponent<Collider>().enabled = true;

        roomLightManager.UpdateRoomLighting();

        characterController.speed = originalSpeed;

        yield return StartCoroutine(FadeToClear());

        enumerator = false;
    }


    private IEnumerator LoadScene(string sceneName) {
        if (!outsideLevel) audioSource.PlayOneShot(doorOpenSFX);

        // Store the originating scene name
        PlayerPrefs.SetString("OriginatingScene", SceneManager.GetActiveScene().name);

        yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(sceneName);

        if (!outsideLevel) audioSource.PlayOneShot(doorCloseSFX);

        yield return null;

        enumerator = false;
    }


    private IEnumerator FadeToBlack() {
        float elapsedTime = 0f;
        Color color = screenTransition.color;

        while (elapsedTime < transitionTime) {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / transitionTime);
            screenTransition.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeToClear() {
        float elapsedTime = 0f;

        Color color = screenTransition.color;

        while (elapsedTime < transitionTime) {
            elapsedTime += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsedTime / transitionTime);
            screenTransition.color = color;
            yield return null;
        }
    }
}
