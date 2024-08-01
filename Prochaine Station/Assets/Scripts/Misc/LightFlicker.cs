using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightFlicker : MonoBehaviour {
    public Transform parentGameObject; // Assign the parent GameObject in the inspector
    public float minFlickerInterval = 0.1f; // Minimum interval between flickers
    public float maxFlickerInterval = 0.5f; // Maximum interval between flickers
    public float minPauseDuration = 1.0f; // Minimum duration of pause
    public float maxPauseDuration = 3.0f; // Maximum duration of pause
    public float bladeRotationSpeed = 10.0f; // Speed at which blades rotate
    private List<Light> pointLights = new List<Light>();
    private List<Transform> fanBlades = new List<Transform>();
    private SkyboxChanger skyboxChanger;

    void Start() {
        skyboxChanger = FindObjectOfType<SkyboxChanger>();
        RefreshLightList();
        FindAllFanBlades(parentGameObject);
    }

    public void RefreshLightList() {
        pointLights.Clear();
        FindAllPointLights(parentGameObject);
        StartFlickering();
    }

    public void FindAllPointLights(Transform parent) {
        foreach (Transform child in parent) {
            Light light = child.GetComponent<Light>();
            if (light != null && light.type == LightType.Point) {
                pointLights.Add(light);
                Debug.Log("Found point light: " + light.name);
            }

            // Recursively search in the child's children
            if (child.childCount > 0) {
                FindAllPointLights(child);
            }
        }
    }

    public void FindAllFanBlades(Transform parent) {
        foreach (Transform child in parent) {
            if (child.name.Contains("Ceiling_Fan")) {
                foreach (Transform grandChild in child) {
                    if (grandChild.name.Contains("Blades")) {
                        fanBlades.Add(grandChild);
                    }
                }
            }

            // Recursively search in the child's children
            if (child.childCount > 0) {
                FindAllFanBlades(child);
            }
        }
    }

    void StartFlickering() {
        foreach (Light light in pointLights) {
            StartCoroutine(FlickerLight(light));
        }
    }

    void Update() {
        RotateFanBlades();
    }

    IEnumerator FlickerLight(Light light) {
        while (true) {
            if (!(skyboxChanger.timeOfDay == "Night" && skyboxChanger.interiorState == "Run-down")) break;

            if (light.gameObject.activeInHierarchy) {
                light.enabled = !light.enabled; // Toggle the light's enabled state
                Renderer renderer = light.GetComponent<Transform>().parent.GetComponent<Renderer>();

                if (renderer != null) {
                    if (light.enabled) {
                        // Enable emission
                        renderer.material.EnableKeyword("_EMISSION");
                    } else {
                        // Disable emission
                        renderer.material.DisableKeyword("_EMISSION");
                    }
                }
            }

            // Wait for a random interval before flickering again
            float flickerInterval = Random.Range(minFlickerInterval, maxFlickerInterval);
            yield return new WaitForSeconds(flickerInterval);

            // Occasionally add a pause
            if (Random.value < 0.2f) { // 20% chance to pause flickering
                float pauseDuration = Random.Range(minPauseDuration, maxPauseDuration);
                yield return new WaitForSeconds(pauseDuration);
            }
        }
    }

    void RotateFanBlades() {
        foreach (Transform blade in fanBlades) {
            blade.Rotate(Vector3.forward, bladeRotationSpeed * Time.deltaTime);
        }
    }
}
