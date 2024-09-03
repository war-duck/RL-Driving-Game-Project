using Godot;
using System;

public partial class Ray : Node2D
{
    private RayCast2D groundDistRay;
    private RayCast2D[] slopeRays;
    public bool shouldDraw = true;
    public override void _Ready()
    {
        groundDistRay = GetNode<RayCast2D>("GroundDistRay");

        slopeRays = new RayCast2D[2];
        slopeRays[0] = GetNode<RayCast2D>("SlopeRayFront");
        slopeRays[1] = GetNode<RayCast2D>("SlopeRayBack");
    }
    
    public float GetGroundDist()
    {
        GlobalRotation = 0;
        if (groundDistRay != null && groundDistRay.IsColliding())
        {
            return groundDistRay.GetCollisionPoint().Y;
        }
        return 0;
    }

    public double GetSlope()
    {
        GlobalRotation = 0;
        if (!slopeRays[0].IsColliding() || !slopeRays[1].IsColliding())
        {
            return 0;
        }
        return -slopeRays[1].GetCollisionPoint().AngleToPoint(slopeRays[0].GetCollisionPoint())*57.2958;
    }
    public override void _Draw()
    {
        if (!shouldDraw)
        {
            return;
        }
        Vector2 offset = new Vector2(0, 50);
        if (groundDistRay != null)
        {
            DrawLine(groundDistRay.Position, groundDistRay.ToLocal(groundDistRay.GetCollisionPoint()), new Color(1, 0, 0), 5, antialiased: true);
        }
        DrawLine(ToLocal(slopeRays[0].GetCollisionPoint() + offset), ToLocal(slopeRays[1].GetCollisionPoint() + offset), new Color(0, 1, 0), 5, antialiased: true);
    }
}
