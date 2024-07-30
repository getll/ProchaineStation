using System;
using UnityEngine;

public class ItemGrabber : MonoBehaviour {
    public float interactionRange = 3f;
    public float dropInteractionRange = 1f;
    public Transform itemHoldPosition;
    public Transform inspectionPosition;
    public Transform cartDropPosition;

    public float swaySpeed = 1.5f;
    public float swayAmount = 0.02f;
    public float throwForce = 10f;

    private Rigidbody heldItem;
    private bool isInspecting = false;
    public PlayerControllerScript playerController;

    void Update() {
        // Check if the player is looking at a grabbable item
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionRange)) {
            GrabbableItem grabbableItem = hit.collider.GetComponent<GrabbableItem>();

            if (grabbableItem != null) {
                // Toggle the outline based on player's proximity
                grabbableItem.ToggleOutline(true);

                // Check if the player presses E to pick up the item
                if (Input.GetKeyDown(KeyCode.E) && heldItem == null && hit.collider.CompareTag("Grabbable")) {
                    // Pick up the item
                    heldItem = hit.collider.GetComponent<Rigidbody>();
                    if (heldItem != null) {
                        PickUpItem(heldItem);
                    }
                }
            } else if (Input.GetKeyDown(KeyCode.E) && heldItem != null && hit.collider.CompareTag("Cart")) {
                // Drop item into cart
                DropItemIntoCart();
                ExitInspectMode();
                heldItem = null;
            } else {
                // No grabbable item in sight or incorrect conditions, disable outline for all grabbable items
                DisableOutlineForAllGrabbableItems();
            }
        } else {
            // No grabbable item in sight, disable outline for all grabbable items
            DisableOutlineForAllGrabbableItems();

            // Check if the player wants to throw the held item
            if (Input.GetKeyDown(KeyCode.E) && heldItem != null) {
                ThrowItem();
                ExitInspectMode();
                heldItem = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.I) && heldItem != null) {
            isInspecting = !isInspecting;
            if (isInspecting) {
                EnterInspectMode();
            } else {
                ExitInspectMode();
            }
        }

        if (heldItem != null) {
            if (!isInspecting) {
                float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
                heldItem.transform.localPosition = new Vector3(0f, sway, 0f);
            } else {
                InspectItem();
            }
        }
    }

    private void PickUpItem(Rigidbody itemRigidbody) {
        if (itemRigidbody.CompareTag("Grabbable")) {
            itemRigidbody.isKinematic = true;
            itemRigidbody.detectCollisions = false;
            itemRigidbody.transform.SetParent(itemHoldPosition);
        }
    }

    private void ThrowItem() {
        if (heldItem != null) {
            heldItem.transform.SetParent(null);
            heldItem.isKinematic = false;
            heldItem.detectCollisions = true;

            RaycastHit hit;
            if (Physics.Raycast(heldItem.transform.position, Vector3.down, out hit, interactionRange)) {
                float distanceToGround = hit.distance;
                if (distanceToGround < 0.1f) {
                    throwForce *= 0.5f;
                }
            }

            isInspecting = false;
            heldItem.AddForce(new Vector3(Camera.main.transform.forward.x, 0.2f, Camera.main.transform.forward.z) * throwForce, ForceMode.Impulse);
            heldItem = null;
        }
    }

    private void EnterInspectMode() {
        playerController.canMove = false;
    }

    private void ExitInspectMode() {
        playerController.canMove = true;
    }

    private void InspectItem() {
        if (inspectionPosition != null && heldItem != null) {
            Collider itemCollider = heldItem.GetComponent<Collider>();
            if (itemCollider != null) {
                Vector3 pivot = itemCollider.bounds.center;

                heldItem.transform.position = inspectionPosition.position;

                float rotationX = Input.GetAxis("Mouse X") * 3f;
                float rotationY = Input.GetAxis("Mouse Y") * 3f;
                float rotationZ = Input.GetAxis("Mouse ScrollWheel") * 3f;

                Vector3 eulerRotation = new Vector3(rotationY, -rotationX, -rotationZ);
                Quaternion deltaRotation = Quaternion.Euler(eulerRotation);
                Quaternion newRotation = deltaRotation * heldItem.transform.rotation;

                heldItem.transform.rotation = newRotation;
                heldItem.transform.RotateAround(pivot, Vector3.forward, rotationZ);
            } else {
                Debug.LogWarning("Collider component not found on the held item.");
                isInspecting = false;
            }
        } else {
            Debug.LogWarning("Inspection position or held item is not set.");
            isInspecting = false;
        }
    }

    private void DropItemIntoCart() {
        heldItem.transform.SetParent(null);
        heldItem.isKinematic = false;
        heldItem.detectCollisions = true;
        heldItem.transform.position = cartDropPosition.position;
    }

    private void DisableOutlineForAllGrabbableItems() {
        GrabbableItem[] grabbableItems = FindObjectsOfType<GrabbableItem>();

        foreach (GrabbableItem grabbableItem in grabbableItems) {
            grabbableItem.ToggleOutline(false);
        }
    }
}
