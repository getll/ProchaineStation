using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomLightManager : MonoBehaviour {
    [Serializable]
    public class Room {
        public string roomName;
        public List<GameObject> lights = new List<GameObject>();
        public bool isActive;
        public bool isPublicArea;
        public bool isDefault;
    }

    [SerializeField]
    public List<Room> rooms;

    private Dictionary<string, Room> roomDictionary;
    private SkyboxChanger skyboxChanger;
    public Room currentRoom;
    public Room defaultRoom;

    private void Start() {
        roomDictionary = new Dictionary<string, Room>();
        skyboxChanger = FindObjectOfType<SkyboxChanger>();

        foreach (var room in rooms) {
            roomDictionary.Add(room.roomName, room);
            if (room.isDefault) {
                currentRoom = room;
                defaultRoom = room;
            }

            // Automatically find lights within the room
            GameObject roomObject = GameObject.Find(room.roomName);
            if (roomObject != null) {
                FindLightsRecursively(roomObject.transform, room);
            }
        }

        // Rotate all "Tree" objects
        RotateTrees();

        UpdateRoomLighting();

        LightSwitch[] lightSwitches = FindObjectsOfType<LightSwitch>();
        foreach (var lightSwitch in lightSwitches) {
            lightSwitch.BroadcastMessage("StartRaycast");
        }

        FindObjectOfType<SkyboxChanger>().BroadcastMessage("UpdateSkybox");
    }

    private void RotateTrees() {
        GameObject[] treeObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var tree in treeObjects) {
            if (tree.name.Contains("Tree")) {
                float randomRotation = UnityEngine.Random.Range(0f, 360f);
                tree.transform.Rotate(0f, randomRotation, 0f);
            }
        }
    }


    private void FindLightsRecursively(Transform parentTransform, Room room) {
        foreach (Transform child in parentTransform) {
            // Assuming light objects have a Light component attached
            if (child.GetComponent<Light>() != null) {
                room.lights.Add(child.gameObject);
            }
            // Recursively search in children
            FindLightsRecursively(child, room);
        }
    }

    public void SetActiveRoom(String roomName) {
        foreach (var room in rooms) {
            if (room.roomName == roomName) {
                currentRoom = room;
            }
        }
    }

    public Room GetRoomByName(string roomName) {
        foreach (Room room in rooms) {
            if (room.roomName == roomName) {
                return room;
            }
        }
        return null; // Or handle the case where no room is found with the provided name
    }

    public void UpdateRoomLighting() {
        foreach (var room in roomDictionary.Values) {
            if (skyboxChanger != null && skyboxChanger.timeOfDay == "Night" && room.isPublicArea) {
                room.isActive = true;
            } else if (skyboxChanger != null && skyboxChanger.timeOfDay == "Day" && room.isPublicArea) {
                room.isActive = false;
            }
        }

        foreach (var room in roomDictionary.Values) {
            SetLightsState(room.lights, false);
        }

        SetLightsState(currentRoom.lights, currentRoom.isActive);

        if (FindObjectOfType<SkyboxChanger>().interiorState == "Run-down" && FindObjectOfType<SkyboxChanger>().timeOfDay == "Night") FindObjectOfType<LightFlicker>().RefreshLightList();
    }

    public static void SetLightsState(List<GameObject> lights, bool state) {
        foreach (var light in lights) {
            light.SetActive(state);

            UpdateEmission(light.transform.parent, state);
        }
    }

    private static void UpdateEmission(Transform lightTransform, bool state) {
        Renderer renderer = lightTransform.GetComponent<Renderer>();
        if (renderer != null) {
            if (state) {
                renderer.material.EnableKeyword("_EMISSION");
            } else {
                renderer.material.DisableKeyword("_EMISSION");
            }
        }
    }
}
