using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    public static PauseMenu Instance { get; private set; }

    public GameObject pauseMenuUI;
    public GameObject mainUI;
    public GameObject playerObject;
    public bool isPaused = false;
    public bool isLevelPausable = true;

    private void Awake() {
        // Ensure only one instance of PauseMenu exists
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        // Hide the pause menu UI initially
        pauseMenuUI.SetActive(false);
    }

    private void Update() {
        // Toggle pause menu on/off when the pause key is pressed
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu() {
        if (!isLevelPausable) return;

        // Toggle pause state
        isPaused = !isPaused;

        // Activate/deactivate pause menu UI
        pauseMenuUI.SetActive(isPaused);
        mainUI.SetActive(!isPaused);

        // Pause/unpause the game
        Time.timeScale = isPaused ? 0f : 1f;

        // Toggle movement script of the player object
        if (playerObject != null) {
            PlayerControllerScript movementScript = playerObject.GetComponent<PlayerControllerScript>();
            if (movementScript != null) {
                movementScript.canMove = !isPaused;
            }
        }

        // Iterate through all audio sources in the scene and pause/unpause them
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources) {
            if (isPaused) {
                audioSource.Pause();
            } else {
                audioSource.UnPause();
            }
        }

        if (isPaused) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Resume() {
        TogglePauseMenu();
    }

    public void QuitGame() {
        Application.Quit();

        // If in the Unity editor, stop playing
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void MainMenu() {
        TogglePauseMenu();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
}
