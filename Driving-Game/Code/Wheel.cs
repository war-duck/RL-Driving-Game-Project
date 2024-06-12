using Godot;
using System;

public partial class Wheel : RigidBody2D
{
	
    public bool isColliding = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
	}

    private void OnBodyEntered(Node body)
    {
        isColliding = true;
        // Console.WriteLine("Entered Wheel: " + body.Name);
    }

    // Called when another body exits the collision area
    private void OnBodyExited(Node body)
    {
        isColliding = false;

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
