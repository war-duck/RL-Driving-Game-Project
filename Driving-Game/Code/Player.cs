using Godot;
using System;
using System.Linq;

public partial class Player : RigidBody2D
{
    public PlayerData playerData;
    Ray rays;
    Font font;
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
    bool dispPlayerParams;

    public override void _Ready()
    {
        font = ResourceLoader.Load<FontFile>("res://Fonts/AgencyFB-Bold.ttf");
        rays = GetTree().GetNodesInGroup("ray").ElementAtOrDefault(0) as Ray;
        wheels = GetTree().GetNodesInGroup("wheel");
        carBody = GetTree().GetNodesInGroup("carBody").ElementAtOrDefault(0) as RigidBody2D;
        deathPolygon = GetTree().GetNodesInGroup("deathPolygon").ElementAtOrDefault(0) as CollisionPolygon2D;
        playerData = new PlayerData();

        dispPlayerParams = true;
        setDispParams();
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
        playerData.AngularVelocity = carBody.AngularVelocity;
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
    private void drawPlayerData()
    {
        rays.QueueRedraw();
        this.QueueRedraw();
    }
    public override void _Draw()
    {
        if(!dispPlayerParams)
            return;
        Vector2 offset = new Vector2(0, -100);
        if(hasDied)
            DrawString(font, offset, "dead", modulate: new Color(1, 0.2f, 0.2f), fontSize: 100);
        else
            DrawString(font, offset, "not dead", modulate: new Color(0, 0.6f, 0), fontSize: 100);
        DrawString(font, 2 * offset, "touching: " + playerData.IsTouchingGround, modulate: new Color(0.8f, 0.8f, 0.8f), fontSize: 100);
        DrawString(font, 3 * offset, "V_ang: " + playerData.AngularVelocity.ToString("F3") , modulate: new Color(0.8f, 0.8f, 0.8f), fontSize: 100);

    }
    public void setDispParams()
    {
        rays.shouldDraw = dispPlayerParams;
    }
}
