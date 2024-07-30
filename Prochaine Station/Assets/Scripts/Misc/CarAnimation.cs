using UnityEngine;
using System.Collections;

public class CarAnimation : MonoBehaviour
{
    public Animator carAnimator; // Reference to the car's Animator
    public float minWaitTime = 2f; // Minimum wait time before the next car animates
    public float maxWaitTime = 5f; // Maximum wait time before the next car animates

    private bool isAnimating = false; // Flag to check if the car is currently animating

    void Start()
    {
        StartCoroutine(CarRoutine());
    }

    private IEnumerator CarRoutine()
    {
        while (true)
        {
            // Wait for a random time before triggering the animation
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // Start the animation if the car is not already animating
            if (!isAnimating)
            {
                isAnimating = true; // Set the flag to indicate the car is animating
                carAnimator.SetTrigger("StartAnimation"); // Trigger the animation
                yield return new WaitForSeconds(carAnimator.GetCurrentAnimatorStateInfo(0).length); // Wait for the animation to finish
                isAnimating = false; // Reset the flag to allow the next car to animate
            }
        }
    }
}
