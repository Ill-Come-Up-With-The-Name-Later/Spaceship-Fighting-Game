using Godot;

public partial class Eventbus : Node
{
    [Signal]
    public delegate void MouseClickEventHandler(Vector2 position);
}
