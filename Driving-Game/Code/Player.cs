using Godot;
using System;

public partial class Player : RigidBody2D
{
	Godot.Collections.Array<Node> wheels, carBody;
	float wheelTorque = 60000;
	float maxSpeed = 50;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		wheels = GetTree().GetNodesInGroup("wheel");
		carBody = GetTree().GetNodesInGroup("carBody");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionPressed("ui_right"))
		{
			foreach (RigidBody2D wheel in wheels)
			{
				if (wheel.AngularVelocity < maxSpeed)
					wheel.ApplyTorqueImpulse(wheelTorque * (float)delta * 60);
			}
			foreach (RigidBody2D car in carBody)
			{
				car.ApplyTorqueImpulse(wheelTorque * (float)delta * 60);
			}
		}
		if (Input.IsActionPressed("ui_left"))
		{
			foreach (RigidBody2D wheel in wheels)
			{
				if (wheel.AngularVelocity > -maxSpeed)
					wheel.ApplyTorqueImpulse(-wheelTorque * (float)delta * 60);
			}
			foreach (RigidBody2D car in carBody)
			{
				car.ApplyTorqueImpulse(-wheelTorque * (float)delta * 60);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
