using UnityEngine;

public class ShoppingCart : MonoBehaviour
{
    public float baseHoldingDistance = 2f; // Base distance to hold the cart in front of the player
    public float proximityDistance = 3f; // Distance required for the player to be close to the cart
    public float lerpSpeed = 5f; // Speed for lerping
    public float pushSpeedMultiplier = 2f; // Speed multiplier for pushing the cart
    public KeyCode holdKey = KeyCode.Q;

    private Transform playerTransform;
    private bool isHolding = false;
    private float originalYPosition;

    void Start()
    {
        playerTransform = Camera.main.transform; // Assuming the camera is the player's head
        originalYPosition = transform.position.y;
    }

    void Update()
    {
        HandleInput();
        MoveCart();
    }

    void HandleInput()
    {
        // Check if the Q key is pressed
        if (Input.GetKeyDown(holdKey))
        {
            isHolding = true;
        }

        // Check if the Q key is released
        if (Input.GetKeyUp(holdKey))
        {
            isHolding = false;
        }
    }

    void MoveCart()
    {
        // If holding and the player is close to the cart, smoothly move the cart towards the desired position
        if (isHolding && IsPlayerClose())
        {
            // Calculate the modified holding distance based on input for smoother forward movement
            float modifiedHoldingDistance = baseHoldingDistance + (Input.GetAxis("Vertical") * pushSpeedMultiplier);

            Vector3 holdingPosition = playerTransform.position + playerTransform.forward * modifiedHoldingDistance;

            // Lerping position
            Vector3 targetPosition = new Vector3(holdingPosition.x, transform.position.y, holdingPosition.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);

            // Calculate the rotation with +90 degrees in the xz plane
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(playerTransform.forward.x, 0f, playerTransform.forward.z), Vector3.up);
            Quaternion desiredRotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y + 90f, 0f);

            // Lerping rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * lerpSpeed);
        }
    }

    bool IsPlayerClose()
    {
        // Check if the player is within the proximity distance
        return Vector3.Distance(playerTransform.position, transform.position) <= proximityDistance;
    }
}
