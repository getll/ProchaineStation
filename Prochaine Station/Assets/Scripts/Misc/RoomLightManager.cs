using System.Collections.Generic;
using UnityEngine;

public class RoomLightManager : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> kitchenLights;

    [SerializeField]
    public List<GameObject> bathroomLights;

    [SerializeField]
    public List<GameObject> readingRoomLights;

    [SerializeField]
    public List<GameObject> bedroomLights;

    private Dictionary<string, List<GameObject>> roomLights;
    public Dictionary<string, bool> roomActiveStates = new Dictionary<string, bool>() {
            { "Kitchen", true },
            { "Bathroom", false },
            { "ReadingRoom", false },
            { "Bedroom", false }
        };
    [SerializeField]
    public string currentActiveRoom = "Kitchen";

    private void Start()
    {
        roomActiveStates = new Dictionary<string, bool>() {
            { "Kitchen", RenderSettings.ambientIntensity < 1 },
            { "Bathroom", false },
            { "ReadingRoom", false },
            { "Bedroom", false }
        };

        roomLights = new Dictionary<string, List<GameObject>>() {
            { "Kitchen", kitchenLights },
            { "Bathroom", bathroomLights },
            { "ReadingRoom", readingRoomLights },
            { "Bedroom", bedroomLights }
        };

        // Initialize the active room state
        UpdateRoomLighting(currentActiveRoom);
    }

    public void SetActiveRoom(string roomName)
    {
        if (!roomLights.ContainsKey(roomName))
        {
            Debug.LogWarning("Room name not found: " + roomName);
            return;
        }

        currentActiveRoom = roomName;
        UpdateRoomLighting(roomName);
    }

    private void UpdateRoomLighting(string roomName)
    {
        foreach (var room in roomLights.Keys)
        {
            if (room == roomName)
            {
                SetLightsState(roomLights[room], roomActiveStates[room]);
            }
            else
            {
                SetLightsState(roomLights[room], false);
            }
        }
    }

    private void SetLightsState(List<GameObject> lights, bool state)
    {
        foreach (var light in lights)
        {
            light.SetActive(state);

            Renderer renderer = light.GetComponent<Transform>().parent.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (state)
                {
                    // Enable emission
                    renderer.material.EnableKeyword("_EMISSION");
                }
                else
                {
                    // Disable emission
                    renderer.material.DisableKeyword("_EMISSION");
                }
            }
        }
    }

    public string GetCurrentActiveRoom()
    {
        return currentActiveRoom;
    }
}
