using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private Transform[] Waypoints;
    [SerializeField]
    private float speed = 2;

    public int CurrentPoint = 0;

    public Vector3 velocity;
    public float minDistance = 0.2f;

    private Vector3 previousPos;

    private void Awake()
    {
        previousPos = transform.position;
    }

    void Update()
    {
        Vector3 dis = Waypoints[CurrentPoint].position - transform.position;

        if (dis.sqrMagnitude < minDistance * minDistance)
        {
            CurrentPoint += 1;
        }
        if (CurrentPoint >= Waypoints.Length)
        {
            CurrentPoint = 0;
        }

        transform.position = Vector3.MoveTowards(transform.position, Waypoints[CurrentPoint].transform.position, speed * Time.deltaTime);

        velocity = previousPos - transform.position;
        previousPos = transform.position;
    }
}