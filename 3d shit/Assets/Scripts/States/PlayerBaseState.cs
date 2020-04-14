public abstract class PlayerBaseState : State
{
    private Controller player;
    public Controller Player => player = player ?? (Controller)owner;
}
