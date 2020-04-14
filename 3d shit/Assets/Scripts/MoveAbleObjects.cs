using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbleObjects : MonoBehaviour
{
    public int mass = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Move(Vector3 direction)
    {
        transform.position += direction/mass;
    }
}
