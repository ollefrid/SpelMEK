using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float moveSpeed = 4;
    [SerializeField]
    private float maxDist = 40;
    [SerializeField]
    private float minDist = 5;
    [SerializeField]
    private struct EnemyArea { };
    [SerializeField]
    private LayerMask collisionLayer;
    [SerializeField]
    private float skinWidth = 0.03f;
    [SerializeField]
    private float staticFriction = 0.45f;
    [SerializeField]
    private float dynamicFriction = 0.25f;

    Vector3 center;
    Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    float distance = 0.0f;

    [SerializeField]
    private bool isAffectedByPlayerGravity = false;
    [SerializeField]
    private bool followPlayer = false;

    [SerializeField]
    private Vector3 playerGravityVector;

    //TODO: Make it so that enemy get it's down velocity from its surface and bind it to it

    void Awake()
    {
        center = transform.GetComponent<BoxCollider>().center;
        Physics.BoxCast(center, transform.localScale, -transform.up, out RaycastHit boxHit);
    }

    void Update()
    {
        playerGravityVector = playerTransform.GetComponent<Controller>().GetGravityVector();
        Collision();
        EnemyMovement();
    }

    void EnemyMovement()
    {
        distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance >= minDist && distance <= maxDist)
        {
            transform.LookAt(playerTransform);

            if (followPlayer)
            {
                velocity += transform.forward * moveSpeed * Time.deltaTime;
            }

            if (isAffectedByPlayerGravity)
            {
                velocity += playerGravityVector * Time.deltaTime;
            }

            transform.position += velocity * Time.deltaTime;

            if (distance <= maxDist)
            {
                //Insert function to do at distance length away from player
                //Or don't, I'm not your dad
            }

        }
    }

    private void Collision()
    {
        while (true)
        {
            Physics.BoxCast(center, transform.localScale, velocity.normalized, out RaycastHit boxHit, transform.rotation, float.MaxValue, collisionLayer);

            if (boxHit.collider != null)
            {
                Vector3 normal = boxHit.normal;

                float distance = skinWidth / Vector3.Dot(velocity.normalized, normal);
                distance += boxHit.distance;

                if (distance > velocity.magnitude * Time.deltaTime)
                {
                    break;
                }

                if (distance > 0)
                {
                    transform.position += velocity.normalized * distance;
                }

                Vector3 velocityBeforeNormalforce = CalculateNormalForce(velocity, normal);
                velocity += velocityBeforeNormalforce;

                Friction(velocityBeforeNormalforce.magnitude);

                /*
                 * TODO: Fix inheriting moving platform velocity 
                 */
                MovingPlatform mov = boxHit.collider.GetComponent<MovingPlatform>();
                if (mov != null)
                {
                    Vector3 velocityProjected = Vector3.ProjectOnPlane(velocity, boxHit.normal);

                    Vector3 diff = mov.velocity - velocityProjected;
                    //Debug.Log(diff + " = " + mov.velocity + " - " + velocityProjected);

                    if (Mathf.Abs((mov.velocity - velocityProjected).magnitude) < staticFriction)
                    {
                        velocity = mov.velocity;
                        Debug.Log("AAAAAAA");
                    }
                    else
                    {
                        velocity = diff;
                        Debug.Log("BBBBBBB");
                    }
                }
            }
            else
            {
                break;
            }
        }
    }

    void Friction(float normalMagnitude)
    {
        if (velocity.magnitude < (normalMagnitude * staticFriction))
        {
            velocity.x = 0;
        }
        else
        {
            velocity += -velocity.normalized * (normalMagnitude * dynamicFriction);
        }
    }

    Vector3 CalculateNormalForce(Vector3 velocity, Vector3 normal)
    {
        if ((Vector3.Dot(velocity, normal) < 0))
        {
            Vector3 projection = Vector3.Dot(velocity, normal) * normal;
            return -projection;
        }
        return Vector3.zero;
    }
}
