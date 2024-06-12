using Godot;
using System;
using System.Linq;

public partial class Player : RigidBody2D
{
    public PlayerData playerData;
    private Ray rays;
    Godot.Collections.Array<Node> wheels;
    InputType currentInput;
    bool inputIsSet;
    RigidBody2D carBody;
    CollisionPolygon2D deathPolygon;
    float wheelTorque = 5000;
    float carTorque = 6000;
    float maxWheelRotSpeed = 60;
    float maxCarRotSpeed = 10;
    bool isColliding = false;
    bool hasDied = false;

    public override void _Ready()
    {
        rays = GetTree().GetNodesInGroup("ray").ElementAtOrDefault(0) as Ray;
        wheels = GetTree().GetNodesInGroup("wheel");
        carBody = GetTree().GetNodesInGroup("carBody").ElementAtOrDefault(0) as RigidBody2D;
        deathPolygon = GetTree().GetNodesInGroup("deathPolygon").ElementAtOrDefault(0) as CollisionPolygon2D;
        playerData = new PlayerData();
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
        
        GetNode<Area2D>("DeathArea").BodyEntered += OnDeathAreaBodyEntered;
    }

    private void OnBodyEntered(Node body) { isColliding = true; }
    private void OnBodyExited(Node body) { isColliding = false; }
    private void OnDeathAreaBodyEntered(Node body) { hasDied = true; }
    private bool isAnythingColliding()
    {
        foreach (Wheel wheel in wheels.Cast<Wheel>())
            if (wheel.isColliding)
                return true;
        return isColliding;
    }

    public override void _PhysicsProcess(double delta)
    {
        rays.rotate(Rotation);
        calcPlayerData();
        drawPlayerData();
        getInput();
        applyInput(delta);
        inputIsSet = false;
    }

    private void getInput()
    {
        if (inputIsSet)
            return;
        if (Input.IsActionPressed("ui_right"))
        {
            currentInput = InputType.Accelerate;
        }
        else if (Input.IsActionPressed("ui_left"))
        {
            currentInput = InputType.Brake;
        }
        else
        {
            currentInput = InputType.None;
        }
    }

    private void applyInput(double delta)
    {
        if (currentInput == InputType.Accelerate && carBody.AngularVelocity > -maxCarRotSpeed)
        {
            carBody.ApplyTorqueImpulse(-carTorque * (float)delta * 60);
        }
        if (currentInput == InputType.Brake && carBody.AngularVelocity < maxCarRotSpeed)
        {
            carBody.ApplyTorqueImpulse(carTorque * (float)delta * 60);
        }

        if (currentInput == InputType.Accelerate)
        {
            foreach (RigidBody2D wheel in wheels.Cast<RigidBody2D>())
            {
                if (wheel.AngularVelocity < maxWheelRotSpeed)
                    wheel.ApplyTorqueImpulse(wheelTorque * (float)delta * 60);
            }
        }
        if (currentInput == InputType.Brake)
        {
            foreach (RigidBody2D wheel in wheels.Cast<RigidBody2D>())
            {
                if (wheel.AngularVelocity > -maxWheelRotSpeed)
                    wheel.ApplyTorqueImpulse(-wheelTorque * (float)delta * 60);
            }
        }
    }

    private void calcPlayerData()
    {
        playerData.Rotation = (int)Rotation;
        playerData.Slope = rays.getSlope();
        playerData.DistToGround = (int)rays.getGroundDist();
        playerData.AngularVelocity = (int)carBody.AngularVelocity;
        playerData.IsTouchingGround = isAnythingColliding();
    }

    public void setCurrentInput(InputType input)
    {
        currentInput = input;
        inputIsSet = true;
    }
    private void printPlayerData()
    {
        GD.Print("Rotation: " + playerData.Rotation);
        GD.Print("Slope: " + playerData.Slope);
        GD.Print("Ground Distance: " + playerData.DistToGround);
        GD.Print("Angular Velocity: " + playerData.AngularVelocity);
        GD.Print("Is Touching Ground: " + playerData.IsTouchingGround);
        GD.Print("Has Died: " + hasDied);
    }
}
