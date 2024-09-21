using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CanvasAlphaLerper : MonoBehaviour
{
    public CanvasGroup[] canvasGroups; // Array of canvas groups for all elements in the canvas
    public RawImage mouseImage; // Reference to the mouse image object
    public float lerpingSpeed = 0.5f; // Speed of lerping
    public string sceneNameToLoad; // Name of the scene to load

    private bool hasMainElementsLoaded = false;
    private bool hasMouseImageLoaded = false;
    private bool isLeftClickPressed = false;

    void Start()
    {
        // Set the alpha of all canvas elements to 0 initially
        foreach (CanvasGroup canvasGroup in canvasGroups)
        {
            canvasGroup.alpha = 0f;
        }

        // Set the alpha of the mouse image to 0 initially
        if (mouseImage != null)
        {
            mouseImage.color = new Color(mouseImage.color.r, mouseImage.color.g, mouseImage.color.b, 0f);
        }

        // Start lerping the alpha of canvas elements
        StartCoroutine(LerpCanvasAlpha());
    }

    IEnumerator LerpCanvasAlpha()
    {
        // Lerping the alpha of main canvas elements
        while (!hasMainElementsLoaded)
        {
            bool allElementsLerped = true;
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, Time.deltaTime * lerpingSpeed);
                if (Mathf.Abs(canvasGroup.alpha - 1f) > 0.01f) // Check if alpha is not close to 1 yet
                {
                    allElementsLerped = false;
                }
            }

            if (allElementsLerped)
            {
                hasMainElementsLoaded = true;
            }

            yield return null;
        }

        // Lerping the alpha of the mouse image with a little delay
        yield return new WaitForSeconds(1f); // Adjust delay as needed

        while (!hasMouseImageLoaded && mouseImage != null)
        {
            mouseImage.color = new Color(mouseImage.color.r, mouseImage.color.g, mouseImage.color.b, Mathf.Lerp(mouseImage.color.a, 1f, Time.deltaTime * lerpingSpeed));

            if (Mathf.Abs(mouseImage.color.a - 1f) < 0.01f) // Check if alpha is close to 1
            {
                hasMouseImageLoaded = true;
            }

            yield return null;
        }
    }

    void Update()
    {
        // Check if left mouse button is pressed
        if (Input.GetMouseButtonDown(0) && hasMouseImageLoaded)
        {
            isLeftClickPressed = true;
        }

        // Fade out all canvas elements if left click is pressed
        if (isLeftClickPressed)
        {
            // Fade out canvas elements
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime * lerpingSpeed);
            }

            // Fade out mouse image
            if (mouseImage != null)
            {
                mouseImage.color = new Color(mouseImage.color.r, mouseImage.color.g, mouseImage.color.b, Mathf.Lerp(mouseImage.color.a, 0f, Time.deltaTime * lerpingSpeed));
            }

            // Check if all canvas elements and mouse image are faded out
            bool allElementsFadedOut = true;
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                if (Mathf.Abs(canvasGroup.alpha) > 0.01f)
                {
                    allElementsFadedOut = false;
                    break;
                }
            }

            if (mouseImage != null && Mathf.Abs(mouseImage.color.a) > 0.01f)
            {
                allElementsFadedOut = false;
            }

            // Change scene if all elements are faded out
            if (allElementsFadedOut)
            {
                SceneManager.LoadScene(sceneNameToLoad);
            }
        }
    }

}
