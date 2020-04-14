using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Tooltip("If this door is open or not")]
    public bool open = false;
    private GameObject[] doorSwitch = new GameObject[5];
    // Start is called before the first frame update
    private void Awake()
    {
 
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Opening()
    {
        bool readyToOpen = true;
        for (int i=0; i<doorSwitch.Length; i++)
        {
            if (doorSwitch[i] == null)
            {
                break;
            }
            if (!doorSwitch[i].GetComponent<DoorSwitch>().activated)
            {
                readyToOpen = false;
            }
            

        }
        if (readyToOpen)
        {
            open = true;
            gameObject.layer = 0;
        }
        else
        {
            open = false;
            gameObject.layer = 8;
        }
           
    }
    public void SetSwitches(GameObject connectedDoorSwitch)
    {
        for (int i =0; i<doorSwitch.Length; i++)
        {
            if (doorSwitch[i] == null)
            {
                doorSwitch[i] = connectedDoorSwitch;
                break;
            }
        }
    }
}
