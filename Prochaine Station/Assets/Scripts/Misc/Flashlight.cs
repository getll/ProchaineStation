using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pizza Doggy's Realistic Flashlight Controller   (more cool assets at: https://pizzadoggy.itch.io/)

public class Flashlight : MonoBehaviour
{
    [Space(10)]
    [Header("Flashlight Settings")]

    [SerializeField] KeyCode toggleKey = KeyCode.F;
    [SerializeField] GameObject CameraObject;
    [SerializeField] GameObject LightSource;
    [SerializeField] AudioClip OnSFX;
    [SerializeField] AudioClip OffSFX;

    private AudioSource audioSource;
    private Vector3 offset;

    bool isOn = false;
    private float speed = 5f;


    void Start()
    {
        CameraObject = Camera.main.gameObject;
        audioSource = GetComponent<AudioSource>();

        LightSource.gameObject.SetActive(false);
        offset = transform.position - CameraObject.transform.position;
    }

    void Update()
    {
        transform.position = CameraObject.transform.position + offset;
        transform.rotation = Quaternion.Slerp(transform.rotation, CameraObject.transform.rotation, speed * Time.deltaTime);

        if (Input.GetKeyDown(toggleKey))
        {
            audioSource.PlayOneShot(OnSFX);
        }

        if (Input.GetKeyUp(toggleKey))
        {
            audioSource.PlayOneShot(OffSFX);

            if (isOn == false)
            {
                LightSource.gameObject.SetActive(true);
                isOn = true;
            }
            else
            {
                LightSource.gameObject.SetActive(false);
                isOn = false;
            }
        }
    }
}
