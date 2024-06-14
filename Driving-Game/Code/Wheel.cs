using Godot;
using System;

public partial class Wheel : RigidBody2D
{
	
    public bool isColliding = false;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
	}

    private void OnBodyEntered(Node body)
    {
        isColliding = true;
    }

    private void OnBodyExited(Node body)
    {
        isColliding = false;

    }

	public override void _Process(double delta)
	{
	}
}
