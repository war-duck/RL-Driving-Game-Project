using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Terrain : StaticBody2D
{
    [ExportGroup("Terrain Generation Parameters")]
    [Export] int yVariation = 500;
    [Export] int sliceWidth = 50;
    [Export] int totalSliceCount = 5000;
    [Export] int noiseSeed = 1;
    [Export] int distToGround = 5000;
    [Export(PropertyHint.Range, "0,20,2")] int difficulty = 3;
    [Export(PropertyHint.Range, "-5,5")] float layerGain = 0.5f;
    [Export(PropertyHint.Range, "0,0.1")] float noiseFrequency = 0.007f;
    [ExportGroup("Children")]
    [Export] Polygon2D terrainPoly;
    [Export] Line2D terrainLine;
    [Export] StaticBody2D collisionBody;
    [Export] AnimatedSprite2D finishFlag;
    List<GasCan> gasCans = new List<GasCan>();
    Vector2[] vertices;
    PackedScene gasCanScene = GD.Load<PackedScene>("res://Scenes/GasCan.tscn");
    public override void _Ready()
    {
        vertices = GenerateTerrainVertices();
        AddGasCans();
        ApplyVerticesToChildren();
    }

    private Vector2[] GenerateTerrainVertices()
    {
        FastNoiseLite noise = new FastNoiseLite();
        List<Vector2> verticesList = new List<Vector2>();
        ConfigureNoise(noise);
        for (int i = 0; i <= totalSliceCount; i++)
        {
            Vector2 point = new Vector2
            {
                X = i * sliceWidth,
                Y = noise.GetNoise1D(i) * yVariation * (1 + difficulty * (float)i / 1000)
            };
            verticesList.Add(point);
        }
        verticesList.Add(verticesList.Last() + new Vector2(200, 0));
        Vector2 enclosingPoint = new Vector2(verticesList.Last().X, -distToGround);
        verticesList.Add(enclosingPoint);
        verticesList.Add(enclosingPoint + new Vector2(2000, 0));
        verticesList.Add(enclosingPoint + new Vector2(2000, 2 * distToGround));
        enclosingPoint = new Vector2(verticesList.First().X - 2000, distToGround);
        verticesList.Add(enclosingPoint);
        verticesList.Add(enclosingPoint + new Vector2(0, -2 * distToGround));
        verticesList.Add(enclosingPoint + new Vector2(2000, -2 * distToGround));
        return verticesList.ToArray();
    }
    private void ApplyVerticesToChildren()
    {
        terrainPoly.Set(Polygon2D.PropertyName.Polygon, vertices);
        terrainLine.Set(Line2D.PropertyName.Points, vertices.Take(totalSliceCount + 2).ToArray());
        CollisionPolygon2D collisionPoly = new CollisionPolygon2D();
        collisionPoly.Set(CollisionPolygon2D.PropertyName.Polygon, vertices.ToArray());
        collisionBody.AddChild(collisionPoly);
        finishFlag.Translate( terrainLine.Points[terrainLine.Points.Length - 2]);
    }
    private void ConfigureNoise(FastNoiseLite noise)
    {
        noise.Seed = noiseSeed;
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        noise.FractalGain = layerGain;
        noise.Frequency = noiseFrequency;
    }
    private void AddGasCans()
    {
        int distBetweenGasCans = (int)(50 * Math.Log2(difficulty + 2));
        int nodeNum = distBetweenGasCans;
        while (nodeNum < vertices.Length)
        {
            Vector2 pos = vertices[nodeNum];
            GasCan gasCan = gasCanScene.Instantiate<GasCan>();
            gasCans.Add(gasCan);
            gasCan.Position = pos + new Vector2(0, -200);
            AddChild(gasCan);
            nodeNum += (int)(distBetweenGasCans + Math.Log10(nodeNum)); // wraz z dystansem i w zależności od trudności
                                                                    // kanistry będą się pojawiać rzadziej
        }
    }
    public void ResetGasCans()
    {
        foreach (var gasCan in gasCans)
        {
            if (IsInstanceValid(gasCan))
                gasCan.QueueFree();
        }
        gasCans.Clear();
        AddGasCans();
    }
}
