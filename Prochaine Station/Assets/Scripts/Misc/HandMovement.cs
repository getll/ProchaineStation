using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pizza Doggy's Realistic Flashlight Controller   (more cool assets at: https://pizzadoggy.itch.io/)

public class HandMovement : MonoBehaviour
{
    [Space(10)]
    [Header("Hand Movement Settings")]
    [SerializeField] float movementRange = 1.5f;
    [SerializeField] float movementSpeed = 2f;
    
    Quaternion initialRotation;


    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        float angle = Mathf.Sin(Time.time * movementSpeed) * movementRange;

        Quaternion targetRotation = initialRotation * Quaternion.Euler(angle, 0f, angle);
        transform.localRotation = targetRotation;
    }
}
