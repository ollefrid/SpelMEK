/*
 Olle frid, olfr3472

 Known problems:
    Moving platforms crashes and/or just does not work TT
    Fix camera when hitting a platform
        watch https://www.youtube.com/watch?v=GC6JuH_gWGU for fix

 Unimplemeted features
    State machine
 */

using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("Speed Settings")]

    [SerializeField]
    [Tooltip("How fast this transform accelerates")]
    private float acceleration = 2;
    [SerializeField]
    [Tooltip("How fast this transform decelerates")]
    private float deceleration = 1;
    [SerializeField]
    [Tooltip("The maximum speed this transform can achieve")]
    private float maxSpeed = 3;
    [SerializeField]
    [Tooltip("The lowest possible speed this transform can achieve before it gets set to zero")]
    private float minimumSpeedCutoff = 0.1f;

    [Header("Friction Settings")]

    [SerializeField]
    [Tooltip("Weird friction 1")]           //TODO: Understand what these frictions/resistances are
    private float staticFriction = 0.45f;
    [SerializeField]
    [Tooltip("Weird friction 2")]           //TODO: Understand what these frictions/resistances are
    private float dynamicFriction = 0.25f;
    [SerializeField]
    [Tooltip("Weird friction 3")]           //TODO: Understand what these frictions/resistances are
    private float airResistance = 0.25f;

    [Header("Jump & Collision Settings")]

    [SerializeField]
    [Tooltip("How much this transform is affected by gravity")]
    private float gravityModifier = 1;
    [SerializeField]
    [Tooltip("How close this transform can get near other transforms")]
    private float skinWidth = 0.03f;
    [SerializeField]
    [Tooltip("How near this transform needs to be to the ground to be able to jump")]
    private float groundCheckDistance = 0.1f;
    [SerializeField]
    [Tooltip("How high this transform can jump")]
    private float jumpHeight = 4;
    [SerializeField]
    [Tooltip("The layer that this transform will collide with")]
    private LayerMask collisionLayer = new LayerMask();

    [Header("Camera Settings")]

    [SerializeField]
    [Tooltip("How sensitive the mouse movements are, corresponding to the camera movements")]
    private float mouseSensitivity = 0.5f;
    [SerializeField]
    [Tooltip("How much the camera can look up expressed in degrees")]
    private float maximumCameraAngle = 90;
    [SerializeField]
    [Tooltip("How much the camera can look down expressed in degrees")]
    private float minimumCameraAngle = -90;
    [SerializeField]
    [Tooltip("How far away the camera is from the player, expressed in 3-dimensional space")]
    private Vector3 cameraOffset = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField]
    [Tooltip("Acticates/ deactivates the third person camera")]
    private bool haveThirdPersonCameraActive = true;
    [SerializeField]
    [Tooltip("In which direction this transform's down is, expressed in 3-dimenaional space")]
    private Vector3 gravityVector = Vector3.down;
    [SerializeField]
    [Tooltip("How many times this transform can flip gravity without touching the ground")]
    private float flipTokens = 1;
    [Header("Non-settings")]
    [SerializeField]
    [Tooltip("How fast the transform moves, expressed in 3-dimenaional space")]
    private Vector3 velocity;

    private Vector3 direction;
    private float pushForce = 10;

    private float height;
    private float radius;
    private Vector3 center;

    private new CapsuleCollider collider;
    private Transform playerCamera;

    private float cameraRadius;

    float rotationX;
    float rotationY;
    float gravityFlipAngle;

    Vector3 gravity;

    Vector3 point1;
    Vector3 point2;

    void Awake()
    {
        playerCamera = Camera.main.transform;
        collider = GetComponent<CapsuleCollider>();
        height = collider.height;
        center = collider.center;
        radius = collider.radius;

        cameraRadius = playerCamera.GetComponent<SphereCollider>().radius;
    }

    void Update()
    {
        MakeSpeed();

        ControlCamera();

        SetGravity();

        gravity = gravityVector * gravityModifier * Time.deltaTime;
        velocity += gravity;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        point1 = transform.position + center + (-gravityVector * ((height / 2) - radius));
        point2 = transform.position + center + (gravityVector * ((height / 2) - radius));

        Vector3 interactDirection = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z);

        Physics.CapsuleCast(point1, point2, radius, interactDirection.normalized, out RaycastHit forwardHit, 0.8f, collisionLayer);

        Debug.Log(interactDirection.normalized + "  <---camer.forward// direction.normalised ----> " + playerCamera.forward);

        if (forwardHit.collider != null)
        {
            Interact(forwardHit);
            Debug.Log("forwardHit.collider is " + forwardHit.collider.name);
        }

        Collision();

        velocity *= Mathf.Pow(airResistance, Time.deltaTime);

        transform.position += velocity * Time.deltaTime;
    }

    private void SetGravity()
    {

        Physics.SphereCast(transform.position, radius, gravityVector, out RaycastHit groundCheck, groundCheckDistance + skinWidth, collisionLayer);

        if (groundCheck.collider != null)
        {
            flipTokens = 1;
        }

        if (Input.GetKeyDown(KeyCode.G) && flipTokens != 0)
        {
            Physics.Raycast(playerCamera.position, playerCamera.transform.forward, out RaycastHit rayHit, 100f, collisionLayer);

            gravityVector = -rayHit.normal;
            transform.up = rayHit.normal;

            flipTokens--;
        }

    }

    private void Collision()
    {
        while (true)
        {
            //Vector3 point1 = transform.position + center + (-gravityVector * ((height / 2) - radius));
            //Vector3 point2 = transform.position + center + (gravityVector * ((height / 2) - radius));

            Physics.CapsuleCast(point1, point2, radius, velocity.normalized, out RaycastHit hit, float.MaxValue, collisionLayer);

            //TITTA
            /*Vector3 interactDirection = new Vector3(playerCamera.forward.y, 0, playerCamera.forward.z);
            Physics.CapsuleCast(point1, point2, radius, interactDirection.normalized, out RaycastHit forwardHit, 1f, collisionLayer);
            Debug.Log(interactDirection.normalized + "  <---camer.forward// direction.normalised ----> " + direction.normalized);

            if (forwardHit.collider != null)
            {
                Interact(forwardHit);
                Debug.Log("forwardHit.collider is " + forwardHit.collider.name);
            }*/

            if (hit.collider != null)
            {
                Vector3 normal = hit.normal;

                float distance = skinWidth / Vector3.Dot(velocity.normalized, normal);
                distance += hit.distance;

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
                MovingPlatform mov = hit.collider.GetComponent<MovingPlatform>();
                if (mov != null)
                {
                    Vector3 velocityProjected = Vector3.ProjectOnPlane(velocity, hit.normal);

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

    public void Interact(RaycastHit hit)
    {
        if (hit.collider.tag == "Switches" && Input.GetKeyDown(KeyCode.E))
        {
            hit.collider.GetComponent<DoorSwitch>().Activate();
        }
        else if (hit.collider.tag == "Moveable Object" && Input.GetKey(KeyCode.E))
        {
            hit.collider.GetComponent<MoveAbleObjects>().Move(direction * pushForce * Time.deltaTime);
        }
    }
    void ControlCamera()
    {
        rotationX -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        rotationY += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, minimumCameraAngle, maximumCameraAngle);

        Quaternion cameraRotation = Quaternion.Euler(rotationX, rotationY, 0);

        //FUNCTIONAL CAMERA ROTATION IN THE X- AND Y-AXISES
        //Quaternion localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.y);

        //FUNCTIONAL CAMERA ROTATION IN ALL AXISES
        Quaternion localRotation = Quaternion.Inverse(transform.rotation) * cameraRotation;

        if (haveThirdPersonCameraActive)
        {
            Vector3 cameraRelationShipVector = localRotation * cameraRotation * cameraOffset;

            playerCamera.position = transform.position + cameraRelationShipVector;

            playerCamera.LookAt(transform, -gravityVector);

            Physics.SphereCast(transform.position, cameraRadius, playerCamera.position, out RaycastHit cameraHit, (cameraRelationShipVector.magnitude - skinWidth), collisionLayer);

            if (cameraHit.collider != null && (cameraRelationShipVector.magnitude - cameraHit.distance) > cameraHit.distance)
            {
                //Debug.Log("cameraHit.collider is " + cameraHit.collider + ", cameraRelationShipVector.magnitude is " + cameraRelationShipVector.magnitude +
                //    ", cameraHit.distance is " + cameraHit.distance);
                playerCamera.position = cameraHit.point;
            }

            //TODO: Fix camera not "sticking" to walls
            if (Physics.Raycast(transform.position, playerCamera.transform.position, float.MaxValue, collisionLayer))
            {
                //Debug.Log(cameraRelationShipVector + ", " + (transform.position - playerCamera.transform.position));
            }
        }
        else
        {
            playerCamera.position = transform.position;
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

    void Jump()
    {
        Physics.SphereCast(transform.position, radius, gravityVector, out RaycastHit groundCheck, groundCheckDistance + skinWidth, collisionLayer);

        if (groundCheck.collider != null)
        {
            velocity += -gravityVector * jumpHeight;
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

    void MakeSpeed()
    {
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        direction = playerCamera.transform.rotation * direction;

        //Physics.SphereCast(transform.position, radius, gravityVector, out RaycastHit groundCheck, groundCheckDistance + skinWidth, collisionLayer);
        Physics.SphereCast(transform.position, radius, gravityVector, out RaycastHit groundCheck, float.MaxValue, collisionLayer);

        if (!haveThirdPersonCameraActive)
        {
            playerCamera.position -= new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        direction = Vector3.ProjectOnPlane(direction, groundCheck.normal).normalized;

        if (direction.magnitude < 0.1)
        {
            Decelerate(direction);
        }
        else
        {
            Accelerate(direction);
        }
    }

    void Accelerate(Vector3 direction)
    {
        velocity += direction * acceleration * Time.deltaTime;

        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }
    }

    void Decelerate(Vector2 direction)
    {
        Vector2 tempVelocity = velocity;

        tempVelocity -= tempVelocity * deceleration * Time.deltaTime;

        Physics.SphereCast(transform.position, radius, gravityVector, out RaycastHit groundCheck, groundCheckDistance + skinWidth, collisionLayer);

        if (groundCheck.collider != null)
        {
            velocity.x = tempVelocity.x;
        }

        if (velocity.magnitude < minimumSpeedCutoff)
        {
            velocity.x = 0;
        }
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetRadius()
    {
        return radius;
    }

    public LayerMask GetCollisionLayer()
    {
        return collisionLayer;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Vector3 GetGravityVector()
    {
        return gravityVector;
    }
}