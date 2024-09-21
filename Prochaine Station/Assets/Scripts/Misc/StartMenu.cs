using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class StartMenu : MonoBehaviour {
    // Assign the scene to load in the inspector
    public string sceneToLoad;

    // Assign the canvas image to fade in/out in the inspector
    public Image fadeImage;

    // Duration of the fade effect
    public float fadeDuration = 1f;

    // This method will be called when the Start button is pressed
    public void StartGame() {
        // Check if a scene name is provided
        if (!string.IsNullOrEmpty(sceneToLoad)) {
            // Start the fade-out process
            StartCoroutine(FadeOutAndStartGame());
        } else {
            Debug.LogError("Scene to load is not set!");
        }
    }

    // Coroutine to handle fading out the canvas image and loading the scene
    private IEnumerator FadeOutAndStartGame() {
        yield return StartCoroutine(FadeCanvas(1)); // Fade to black
        SceneManager.LoadScene(sceneToLoad);   // Load the specified scene
    }

    // This method will be called when the Quit button is pressed
    public void QuitGame() {
        Application.Quit();

        // If in the Unity editor, stop playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // This method will handle the fade-in/fade-out effect
    private IEnumerator FadeCanvas(float targetAlpha) {
        fadeImage.enabled = true;

        float startAlpha = fadeImage.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
            yield return null;
        }

        // Ensure the target alpha is set after the loop
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);

        fadeImage.enabled = targetAlpha == 1;
    }

    // Call this method to fade in the canvas image at the start
    private void Start() {
        StartCoroutine(FadeCanvas(0)); // Fade from black
    }
}
