using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    [Tooltip("if the switch is active or not")]
    public bool activated = false;
    //public bool connectedKey;
    [Tooltip("not yet implemented skip this one")]
    public GameObject[] connectedSwitchObjects;
    [Tooltip("all the doors you want this switch to open")]
    public GameObject[] connectedDoors;
    [Tooltip("A reference to the gameobject with the GameController script")]
    public GameObject gameController;

    private Material activatedMaterial;
    private Material deActivatedMaterial;
    // Start is called before the first frame update
    public bool tester = false;
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        activatedMaterial = gameController.GetComponent<GameController>().switchActivatedMaterial;
        deActivatedMaterial = gameController.GetComponent<GameController>().switchDeactivatedMaterial;

        if (activated)
        {
            this.gameObject.GetComponent<MeshRenderer>().material = activatedMaterial;
        }
        else
        {
            Debug.Log("hej");
            this.gameObject.GetComponent<MeshRenderer>().material = deActivatedMaterial;
        }

        for (int i = 0; i<connectedDoors.Length; i++)
        {
            connectedDoors[i].GetComponent<Door>().SetSwitches(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tester == false)
        {
            Activate();
            tester = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OTE tag "+ other.tag);
        string othersTag = other.tag;
        if (tag == "Player")
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (!activated)
        {
            /*for (int i = 0; i < connectedSwitchObjects.Length; i++)
            {
                bool connectedObjectOpen = connectedSwitchObjects[i].GetComponent<DoorSwitch>().activated;
                //connectedSwitchObjects[i].GetComponent<DoorSwitch>().activated = !connectedObjectOpen;
                Debug.Log("fungerar");
                connectedSwitchObjects[i].GetComponent<DoorSwitch>().DoorTrigger(!connectedObjectOpen);
            }*/
            activated = true;
            DoorTrigger(activated);
            this.gameObject.GetComponent<MeshRenderer>().material = activatedMaterial;
        }
        else
        {
            activated = false;
            DoorTrigger(activated);
            this.gameObject.GetComponent<MeshRenderer>().material = deActivatedMaterial;
        }        
    }
    public void DoorTrigger(bool activated)
    {
        for (int j = 0; j < connectedDoors.Length; j++)
        {
            connectedDoors[j].GetComponent<Door>().Opening();
        }
    }
}
