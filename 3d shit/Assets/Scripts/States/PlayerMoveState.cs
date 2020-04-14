using UnityEngine;

[CreateAssetMenu(menuName = "PlayerState/MoveState")]
public class PlayerMoveState : PlayerBaseState
{
    public float Speed;
    public float MaxWalkSpeed;
    public float MinWalkSpeed;

    public override void Enter()
    {
        Debug.Log("Enter Move State");
    }
    public override void Run()
    {
        if (Player.GetVelocity().magnitude < MinWalkSpeed)
        {
            //stateMachine.TransitionTo<StandingState>();
        }

        if (Player.GetVelocity().magnitude > Player.GetMaxSpeed())
        {
            //stateMachine.TransitionTo<RunningState>();
        }

        if (!Physics.SphereCast(Player.transform.position, Player.GetRadius(), Vector3.down, out RaycastHit groundCheck, 10f, Player.GetCollisionLayer()))
        {
            //stateMachine.TransitionTo<FallingState>();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //stateMachine.TransitionTo<JumpState>();
        }

        Vector3 movement = (Input.GetAxis("Horizontal") * Vector3.right + Input.GetAxis("Vertical") * Vector3.forward).normalized * Speed * Time.deltaTime;
        Player.transform.position += movement;
    }
}
