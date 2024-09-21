using UnityEngine;
using UnityEngine.UI;

public class PlayerSequenceController : MonoBehaviour {
    public Animator playerAnimator; // Animator for the player object
    public GameObject promptUI; // UI element to prompt player to click
    public GameObject newspaperObject; // Newspaper object to animate
    public GameObject targetObject; // The object whose script will be activated
    public Texture transparentLogo;

    private bool isPromptActive = false;

    private void Start() {
        promptUI.SetActive(false); // Hide the UI prompt initially
        StartPlayerAnimation();
    }

    public void StartPlayerAnimation() {
        playerAnimator.SetTrigger("StartTransformAnimation");
    }

    // Call this method at the end of the player transform animation via animation event
    public void OnPlayerAnimationFinished() {
        promptUI.SetActive(true);
        isPromptActive = true;
    }

    private void Update() {
        if (isPromptActive && Input.GetMouseButtonDown(0)) {
            OnPlayerClicked();
        }
    }

    private void OnPlayerClicked() {
        // Hide the prompt UI
        promptUI.SetActive(false);
        isPromptActive = false;

        // Trigger the newspaper fly animation or physics
        Animator newspaperAnimator = newspaperObject.GetComponent<Animator>();
        if (newspaperAnimator != null) {
            newspaperAnimator.SetTrigger("Fly");
        }

        FindObjectOfType<CameraShakeWithAudio>().enabled = true;

        // Start the next camera animation
        playerAnimator.SetTrigger("NextTransformAnimation");

        GameObject.Find("StudioLogo").GetComponent<RawImage>().texture = transparentLogo;
    }

    // Call this method at the end of the camera animation via animation event
    public void OnCameraAnimationFinished() {
        // Stabilize camera (e.g., return to idle state or specific transform)
        playerAnimator.SetTrigger("Stabilize");
    }
}
