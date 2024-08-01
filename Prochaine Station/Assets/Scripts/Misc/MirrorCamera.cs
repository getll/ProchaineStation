using UnityEngine;

public class MirrorCamera : MonoBehaviour {
    [SerializeField]
    private Camera playerCamera; // Reference to the player's camera
    [SerializeField]
    private Transform mirrorPlane; // Reference to the mirror's transform
    [SerializeField]
    private Camera mirrorCamera; // Reference to the mirror's camera

    [Range(0.1f, 2f)]
    [SerializeField]
    private float zoomFactor = 1f; // Factor to control zoom level

    private void LateUpdate() {
        // Reflect the player's position relative to the mirror plane
        Vector3 mirrorNormal = mirrorPlane.forward;
        Vector3 playerToMirror = playerCamera.transform.position - mirrorPlane.position;
        Vector3 reflectedPosition = playerCamera.transform.position - 2 * Vector3.Dot(playerToMirror, mirrorNormal) * mirrorNormal;

        // Set the mirror camera's position
        transform.position = reflectedPosition;

        // Reflect the player's rotation relative to the mirror plane
        Vector3 reflectedForward = Vector3.Reflect(playerCamera.transform.forward, mirrorNormal);
        Vector3 reflectedUp = Vector3.Reflect(playerCamera.transform.up, mirrorNormal);
        Quaternion reflectedRotation = Quaternion.LookRotation(reflectedForward, reflectedUp);

        // Set the mirror camera's rotation to look in the reflected forward direction and with the reflected up direction
        transform.rotation = reflectedRotation * Quaternion.Euler(0, 180, 0); // Flip the rotation along the y-axis

        // Adjust the field of view for the mirror camera to control the zoom level
        mirrorCamera.fieldOfView = playerCamera.fieldOfView * zoomFactor;
    }
}
