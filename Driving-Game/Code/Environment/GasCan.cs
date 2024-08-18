using Godot;
using System;

public partial class GasCan : Area2D
{
    [Export]
    public AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        animationPlayer.AnimationFinished += (animName) => QueueFree();
    }

    private void OnBodyEntered(object body)
    {
        if (body is Player player)
        {
            animationPlayer.Play("DespawnGasCan");
            player.EmitSignal("GasCanCollected");
        }
    }
}
