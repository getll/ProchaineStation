using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarMover : MonoBehaviour {
    [System.Serializable]
    public class Route {
        public List<Transform> waypoints; // List of waypoints for the route
    }

    [SerializeField] private Route[] routes; // Array of routes
    [SerializeField] private GameObject[] cars; // Array of car objects
    [SerializeField] private float minInterval = 2f; // Minimum time interval between movements
    [SerializeField] private float maxInterval = 5f; // Maximum time interval between movements
    [SerializeField] private float carSpeed = 5f; // Fixed speed of the car
    [SerializeField] private float pauseDistance = 5f; // Distance within which the car pauses for the player or another car

    [SerializeField] private AudioClip engineLoopClip; // Engine loop sound clip
    [SerializeField] private AudioClip hornClip; // Horn sound clip
    [SerializeField] private float hornIntervalMin = 3f; // Minimum interval between horn sounds
    [SerializeField] private float hornIntervalMax = 10f; // Maximum interval between horn sounds

    private float[] movementTimers;
    private bool[] isCarMoving; // Array to track whether each car is currently moving
    private bool[] isCarBlocked; // Array to track whether each car is blocked
    private AudioSource[] audioSources; // Array to store the audio sources for each car
    private GameObject player; // Reference to the player object
    private Dictionary<GameObject, Route> carRoutes; // Dictionary to map cars to their current routes
    private HashSet<Route> activeRoutes; // Set to track active routes

    void Start() {
        InitializeMovementTimers();
        player = GameObject.FindWithTag("Player"); // Find the player object by tag
        carRoutes = new Dictionary<GameObject, Route>(); // Initialize the dictionary
        activeRoutes = new HashSet<Route>(); // Initialize the set for active routes
        InitializeAudioSources();
    }

    void Update() {
        UpdateMovementTimers();
    }

    void InitializeMovementTimers() {
        movementTimers = new float[routes.Length];
        isCarMoving = new bool[cars.Length]; // Initialize the moving state array
        isCarBlocked = new bool[cars.Length]; // Initialize the blocked state array
        audioSources = new AudioSource[cars.Length]; // Initialize the audio sources array

        for (int i = 0; i < routes.Length; i++) {
            movementTimers[i] = Random.Range(minInterval, maxInterval);
        }
    }

    void InitializeAudioSources() {
        for (int i = 0; i < cars.Length; i++) {
            AudioSource audioSource = cars[i].GetComponent<AudioSource>();
            audioSource.loop = true;
            audioSources[i] = audioSource;
        }
    }

    void UpdateMovementTimers() {
        // Check if the time of day is "Day"
        if (FindObjectOfType<SkyboxChanger>().timeOfDay == "Day") {
            for (int i = 0; i < routes.Length; i++) {
                movementTimers[i] -= Time.deltaTime;
                if (movementTimers[i] <= 0) {
                    // Pick a random car
                    int carIndex = Random.Range(0, cars.Length);
                    if (!isCarMoving[carIndex]) { // Check if the car is not moving
                        Route selectedRoute = routes[i];

                        // Check if the route is already active
                        if (!activeRoutes.Contains(selectedRoute)) {
                            // Assign the route to the car in the dictionary
                            carRoutes[cars[carIndex]] = selectedRoute;
                            activeRoutes.Add(selectedRoute);

                            // Start the engine sound
                            StartCarEngine(carIndex);

                            // Move the car along the route
                            StartCoroutine(MoveCar(cars[carIndex], selectedRoute, carIndex));

                            // Reset the timer for the route
                            movementTimers[i] = Random.Range(minInterval, maxInterval);
                        }
                    }
                }
            }
        }
    }

    void StartCarEngine(int carIndex) {
        AudioSource audioSource = audioSources[carIndex];
        audioSource.clip = engineLoopClip;
        audioSource.Play();
    }

    void StopCarEngine(int carIndex) {
        AudioSource audioSource = audioSources[carIndex];
        audioSource.Stop();
    }

    IEnumerator MoveCar(GameObject car, Route route, int carIndex) {
        isCarMoving[carIndex] = true; // Mark the car as moving
        isCarBlocked[carIndex] = false; // Reset blocked state

        // Teleport the car to the first waypoint of the route
        car.transform.position = route.waypoints[0].position;

        for (int i = 1; i < route.waypoints.Count; i++) { // Start from the second waypoint
            Transform waypoint = route.waypoints[i];
            Vector3 startPosition = car.transform.position;
            Vector3 targetPosition = waypoint.position;

            // Calculate the direction to the target waypoint
            Vector3 directionToTarget = (targetPosition - startPosition).normalized;

            // Calculate the rotation that should be applied to the car to face the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            float distanceToTarget = Vector3.Distance(startPosition, targetPosition);

            while (distanceToTarget > 0.1f) { // Move until the car is very close to the target
                // Check if the player is near the car's path or if another car is near
                if (IsPlayerNear(car.transform.position, targetPosition) || IsAnotherCarNear(car, route, targetPosition)) {
                    isCarBlocked[carIndex] = true;
                    StartCoroutine(PlayHorn(carIndex));
                    yield return new WaitForSeconds(Random.Range(hornIntervalMin, hornIntervalMax));
                    yield return null;
                    continue;
                } else {
                    isCarBlocked[carIndex] = false;
                    StopCoroutine(PlayHorn(carIndex));
                }

                // Smoothly rotate the car towards the target direction
                car.transform.rotation = Quaternion.Lerp(car.transform.rotation, targetRotation, Time.deltaTime * carSpeed);

                // Move the car towards the target position
                float step = carSpeed * Time.deltaTime; // Calculate the step size
                car.transform.position = Vector3.MoveTowards(car.transform.position, targetPosition, step);

                distanceToTarget = Vector3.Distance(car.transform.position, targetPosition);
                yield return null;
            }

            // Ensure the car ends up at the target position and is facing the correct direction
            car.transform.position = targetPosition;
            car.transform.rotation = targetRotation;
        }

        StopCarEngine(carIndex); // Stop the engine sound when the car stops moving
        isCarMoving[carIndex] = false; // Mark the car as not moving anymore
        activeRoutes.Remove(route); // Remove the route from active routes once the car has completed the route
    }

    IEnumerator PlayHorn(int carIndex) {
        AudioSource audioSource = audioSources[carIndex];
        while (isCarBlocked[carIndex]) {
            audioSource.PlayOneShot(hornClip);
            yield return new WaitForSeconds(Random.Range(hornIntervalMin, hornIntervalMax));
        }
    }

    bool IsPlayerNear(Vector3 carPosition, Vector3 targetPosition) {
        // Check if the player is within a certain distance from the car's path
        Vector3 directionToTarget = (targetPosition - carPosition).normalized;
        Vector3 directionToPlayer = (player.transform.position - carPosition).normalized;

        float distanceToPlayer = Vector3.Distance(carPosition, player.transform.position);
        float angleToPlayer = Vector3.Angle(directionToTarget, directionToPlayer);

        return distanceToPlayer < pauseDistance && angleToPlayer < 45f; // Adjust angle as needed
    }

    bool IsAnotherCarNear(GameObject currentCar, Route route, Vector3 currentTargetPosition) {
        foreach (GameObject car in cars) {
            if (car != currentCar && isCarMoving[System.Array.IndexOf(cars, car)]) {
                // Check if the other car is on the same route using the dictionary
                if (carRoutes.ContainsKey(car) && carRoutes[car] == route) {
                    Vector3 otherCarPosition = car.transform.position;
                    Vector3 directionToOtherCar = (otherCarPosition - currentCar.transform.position).normalized;
                    Vector3 directionToTarget = (currentTargetPosition - currentCar.transform.position).normalized;

                    // Check if the other car is ahead and within a safe distance
                    if (Vector3.Distance(currentCar.transform.position, otherCarPosition) < pauseDistance &&
                        Vector3.Dot(directionToTarget, directionToOtherCar) > 0) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
