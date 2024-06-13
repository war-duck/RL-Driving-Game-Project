using Godot;
using System;

public partial class Ray : Node2D
{
    public RayCast2D groundDistRay;
    public RayCast2D[] slopeRays;
    public bool shouldDraw = true;
    public override void _Ready()
    {
        var groundDistNodes = GetTree().GetNodesInGroup("groundDist");
        if (groundDistNodes.Count > 0)
        {
            groundDistRay = groundDistNodes[0] as RayCast2D;
        }
        Console.WriteLine( GetTreeStringPretty());
        var slopeRayNodes = GetTree().GetNodesInGroup("slopeRay");
        slopeRays = new RayCast2D[slopeRayNodes.Count];
        for (int i = 0; i < slopeRayNodes.Count; i++)
        {
            slopeRays[i] = slopeRayNodes[i] as RayCast2D;
        }
    }
    
    public float getGroundDist()
    {
        if (groundDistRay != null && groundDistRay.IsColliding())
        {
            return groundDistRay.GetCollisionPoint()[1];
        }
        return 0;
    }

    public int getSlope()
    {
        if (!slopeRays[0].IsColliding() || !slopeRays[1].IsColliding())
        {
            return 0;
        }
        return -(int)(slopeRays[1].GetCollisionPoint().AngleToPoint(slopeRays[0].GetCollisionPoint())*57.2958);
    }

    public void rotate(float rotation)
    {
        this.Rotation = -rotation;
    }

    public override void _Process(double delta)
    {
        // QueueRedraw();
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
            DrawLine(groundDistRay.Position, groundDistRay.ToLocal(groundDistRay.GetCollisionPoint()), new Color(1, 0, 0), 5);
        }
        DrawLine(ToLocal(slopeRays[0].GetCollisionPoint() + offset), ToLocal(slopeRays[1].GetCollisionPoint() + offset), new Color(0, 1, 0), 5);
    }
}
