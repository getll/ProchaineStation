using UnityEngine;
using System.Collections;

public class GrabbableItem : MonoBehaviour {
    private Outline outline;

    void Start() {
        // Find and store the Outline component
        outline = GetComponent<Outline>();
        if (outline == null) {
            // If Outline component is not found, add it
            outline = gameObject.AddComponent<Outline>();
        }

        // Disable the outline by default
        outline.enabled = false;
    }

    // Toggle the outline based on the specified state
    public void ToggleOutline(bool state) {
        if (outline != null) {
            StartCoroutine(ToggleOutlineCoroutine(state));
        }
    }

    private IEnumerator ToggleOutlineCoroutine(bool state) {
        yield return new WaitForSeconds(0.1f);
        outline.enabled = state;
    }
}
