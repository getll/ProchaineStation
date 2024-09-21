using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepClips;
    public float movementThreshold = 0.1f;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (characterController.velocity.magnitude > movementThreshold)
        {
            PlayFootstepSound();
        }
    }

    void PlayFootstepSound()
    {
        if (!footstepAudioSource.isPlaying)
        {
            footstepAudioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
        }
    }
}
