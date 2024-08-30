using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicRun : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene("BuskerStation", LoadSceneMode.Single);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("CoworkerStation", LoadSceneMode.Single);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene("GroceryStation", LoadSceneMode.Single);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene("RatStation", LoadSceneMode.Single);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            foreach (GameObject blocker in GameObject.FindGameObjectsWithTag("Blocker"))
            {
                if (blocker.GetComponent<MeshRenderer>().enabled)
                {
                    blocker.GetComponent<MeshRenderer>().enabled = false;
                }
                else 
                {
                    blocker.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
    }
}
