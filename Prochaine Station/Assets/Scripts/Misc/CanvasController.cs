using UnityEngine;

public class CanvasController : MonoBehaviour
{
    Canvas canvas;

    void Start()
    {
        // Get the Canvas component
        canvas = GetComponent<Canvas>();
    }

    void Update()
    {
        // Check if the player is moving (you can replace this condition with your own)
        bool isPlayerMoving = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;

        // Set the canvas active or inactive based on the condition
        canvas.enabled = !isPlayerMoving;
    }
}
