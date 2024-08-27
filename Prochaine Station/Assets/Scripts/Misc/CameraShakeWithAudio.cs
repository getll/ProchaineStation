using UnityEngine;
using System.Collections;

public class CameraShakeWithAudio : MonoBehaviour {
    public AudioSource audioSource;
    public AudioClip initialAudioClip;
    public AudioClip subsequentAudioClip;
    public float shakeDelay = 1.5f; // Time before shaking starts
    public float shakeDuration = 2.0f; // Duration of camera shake
    public float shakeMagnitude = 0.5f; // Maximum magnitude of the shake effect
    public float fadeOutDuration = 1.0f; // Duration of audio fade out
    public Transform trainObject;
    public Transform startTransform;
    public Transform endTransform;
    public float trainSpeed = 1.0f; // Speed of the train movement
    public GameObject canvas;

    private bool isInitialPlay = true;
    private Vector3 originalPosition;

    void Start() {
        originalPosition = transform.localPosition;
        PlayShakeEffect();
        StartCoroutine(MoveTrain());
    }

    public void PlayShakeEffect() {
        if (isInitialPlay) {
            audioSource.clip = initialAudioClip;
            isInitialPlay = false;
        } else {
            audioSource.clip = subsequentAudioClip;
        }

        audioSource.Play();
        StartCoroutine(ShakeSequence());
        StartCoroutine(MoveTrain());
    }

    private IEnumerator ShakeSequence() {
        // Wait for the delay before shaking
        yield return new WaitForSeconds(shakeDelay);

        canvas.SetActive(false);

        // Shake the camera
        yield return StartCoroutine(ShakeCamera());

        // Start fading out the audio
        StartCoroutine(FadeOutAudio());

        // Reset camera position to the original position
        transform.localPosition = originalPosition;
    }

    private IEnumerator ShakeCamera() {
        yield return new WaitForSeconds(2f);

        float elapsedTime = 0.0f;

        while (elapsedTime < shakeDuration) {
            // Lerp intensity based on the elapsed time
            float intensity = Mathf.Lerp(0, shakeMagnitude, elapsedTime / (shakeDuration / 2));
            if (elapsedTime > shakeDuration / 2) {
                intensity = Mathf.Lerp(shakeMagnitude, 0, (elapsedTime - shakeDuration / 2) / (shakeDuration / 2));
            }

            // Randomize the camera's local position within the lerped range
            Vector3 randomOffset = Random.insideUnitSphere * intensity;
            transform.localPosition = originalPosition + randomOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the camera returns to its original position
        transform.localPosition = originalPosition;
    }

    private IEnumerator FadeOutAudio() {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeOutDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Reset volume for next play
    }

    private IEnumerator MoveTrain() {
        // Move train from start to end transform
        float journey = 0.0f;
        while (journey <= 1.0f) {
            journey += Time.deltaTime * trainSpeed;

            if (journey > 0.5f) canvas.SetActive(true);

            trainObject.position = Vector3.Lerp(startTransform.position, endTransform.position, journey);
            yield return null;
        }



        // Wait for a moment at the start
        yield return new WaitForSeconds(1.0f);
    }
}
