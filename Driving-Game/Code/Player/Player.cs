using Godot;

public partial class Player : RigidBody2D
{
    public PlayerData playerData;
    public Ray rays;
    ProgressBar fuelBar;
    Font font;
    Wheel[] wheels;
    InputType currentInput;
    CollisionPolygon2D deathPolygon;
    float wheelTorque = 3000;
    float carTorque = 4500;
    float maxWheelRotSpeed = 60;
    float maxCarRotSpeed = 10;
    bool isColliding = false;
    bool hasDied = false;
    bool dispPlayerParams;
    float fuelLevel = 100;
    float fuelBurnRate = 0.1f;
    [Signal]
    public delegate void GasCanCollectedEventHandler();

    public override void _Ready()
    {
        rays = GetNode<Ray>("Ray");
        wheels = new Wheel[2];
        wheels[0] = GetNode<Wheel>("WheelHolderFront/Wheel");
        wheels[1] = GetNode<Wheel>("WheelHolderBack/Wheel");
        deathPolygon = GetNode<CollisionPolygon2D>("DeathArea/DeathPolygon");
        fuelBar = GetNode<ProgressBar>("FuelBar/ProgressBar");
        playerData = new PlayerData();
        dispPlayerParams = true;
        SetDispParams();
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
        GetNode<Area2D>("DeathArea").BodyEntered += OnDeathAreaBodyEntered;
        GasCanCollected += () => { fuelLevel = 100f; };
        playerData.GlobalPositionX = GlobalPosition.X;
    }

    private void OnBodyEntered(Node body) { isColliding = true; }
    private void OnBodyExited(Node body) { isColliding = false; }
    private void OnDeathAreaBodyEntered(Node body) { hasDied = true; }
    private bool IsAnythingColliding()
    {
        foreach (Wheel wheel in wheels)
            if (wheel.isColliding)
                return true;
        return isColliding;
    }

    public override void _PhysicsProcess(double delta)
    {
    }
    public void MovePlayer(double delta)
    {
        HandleFuel();
        CalcPlayerData();
        DrawPlayerData();
        GetInput();
        ApplyInput(delta);
    }

    private void GetInput()
    {                // for manual override
        if (!Input.IsActionPressed("ui_accept"))
        {
            return;
        }
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

    private void ApplyInput(double delta)
    {
        if (currentInput == InputType.Accelerate)
        {
            foreach (RigidBody2D wheel in wheels)
            {
                if (wheel.AngularVelocity < maxWheelRotSpeed)
                    wheel.ApplyTorqueImpulse(wheelTorque * (float)delta * 60);
            }
            if (this.AngularVelocity > -maxCarRotSpeed)
                this.ApplyTorqueImpulse(-carTorque * (float)delta * 60);
        }
        if (currentInput == InputType.Brake)
        {
            foreach (RigidBody2D wheel in wheels)
            {
                if (wheel.AngularVelocity > -maxWheelRotSpeed)
                    wheel.ApplyTorqueImpulse(-wheelTorque * (float)delta * 60);
            }
            if (this.AngularVelocity < maxCarRotSpeed)
                this.ApplyTorqueImpulse(carTorque * (float)delta * 60);
        }
    }

    private void CalcPlayerData()
    {
        playerData.GlobalPositionX = (int)GlobalPosition.X;
        playerData.Rotation = (int)Rotation;
        playerData.Slope = rays.GetSlope();
        playerData.DistToGround = (int)rays.GetGroundDist();
        playerData.AngularVelocity = (int)(this.AngularVelocity*100);
        playerData.IsTouchingGround = IsAnythingColliding();
        playerData.HasDied = hasDied;
    }

    public void SetCurrentInput(InputType input)
    {
        currentInput = input;
    }
    private void DrawPlayerData()
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
        DrawString(font, 2 * offset, "touching: " + playerData.IsTouchingGround, modulate: new Color(0.5f, 0.5f, 0.5f), fontSize: 100);
        DrawString(font, 3 * offset, "V_ang: " + playerData.AngularVelocity.ToString("F3") , modulate: new Color(0.5f, 0.5f, 0.5f), fontSize: 100);
        DrawString(font, 4 * offset, "FuelLevel: " + fuelLevel.ToString("F1") , modulate: new Color(0.5f, 0.5f, 0.5f), fontSize: 100);
    }
    public void SetDispParams()
    {
        rays.shouldDraw = dispPlayerParams;
        font = ResourceLoader.Load<FontFile>("res://Fonts/AgencyFB-Bold.ttf");
    }
    private void HandleFuel()
    {
        if (fuelLevel <= 0)
        {
            hasDied = true;
        }
        else
        {
            fuelLevel -= fuelBurnRate;
            fuelBar.Value = fuelLevel;
        }
    }
}
