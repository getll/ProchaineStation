using UnityEngine;

public class StructureSpawner : MonoBehaviour {
    public enum OriginDestinationType { Earthy, Urban } // Enum to define the type
    public OriginDestinationType currentType;

    public GameObject player; // The player object
    public GameObject earthyStructure; // Reference to the Earthy Structure
    public Transform earthySpawnPoint; // Spawn point in the Earthy Structure

    public GameObject urbanStructure; // Reference to the Urban Structure
    public Transform urbanSpawnPoint; // Spawn point in the Urban Structure

    void Start() {
        // Deactivate both structures initially
        earthyStructure.SetActive(false);
        urbanStructure.SetActive(false);

        if (!System.Enum.IsDefined(typeof(OriginDestinationType), currentType)) {
            currentType = OriginDestinationType.Earthy;
        }

        // Determine which structure to activate and where to spawn the player
        HandleStructureActivation();
    }

    void HandleStructureActivation() {
        if (currentType == OriginDestinationType.Earthy) {
            // Activate Earthy structure and move player to the Earthy spawn point
            earthyStructure.SetActive(true);
            player.transform.position = earthySpawnPoint.position;
            player.transform.rotation = earthySpawnPoint.rotation;
        } else if (currentType == OriginDestinationType.Urban) {
            // Activate Urban structure and move player to the Urban spawn point
            urbanStructure.SetActive(true);
            player.transform.position = urbanSpawnPoint.position;
            player.transform.rotation = urbanSpawnPoint.rotation;
        }
    }
}
