public partial class DroneController : EnemyController
{
    public override void _Process(double delta)
    {
        if (!IsInstanceValid(ParentShip))
            ChangeState(Profile.InitialState);
        
        if (Dead || !IsInstanceValid(ParentShip))
            return;
        
        TimeInState += delta;
        CurrentState?.Process(this, delta);
        EvaluateTransitions(delta);
    }
}