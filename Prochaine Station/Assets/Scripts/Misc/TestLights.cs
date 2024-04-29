using System.Collections.Generic;
using UnityEngine;

public class ToggleGameObjects : MonoBehaviour
{
    public List<GameObject> objectsToToggle = new List<GameObject>();

    void Update()
    {
        // Check if the player presses the "F" key
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Toggle the state of game objects
            ToggleObjectsState();
        }
    }

    void ToggleObjectsState()
    {
        // Loop through each object in the list
        foreach (GameObject obj in objectsToToggle)
        {
            // Toggle the active state of the object based on the objectsActive variable
            obj.SetActive(!obj.activeSelf);
        }
    }
}
