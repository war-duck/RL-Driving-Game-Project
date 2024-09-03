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
    [Export] int noiseSeed = 0;
    [Export] int distToGround = 5000;
    [Export(PropertyHint.Range, "0,20,2")] int difficulty = 3;
    [Export(PropertyHint.Range, "-5,5")] float layerGain = 0.5f;
    [Export(PropertyHint.Range, "0,0.1")] float noiseFrequency = 0.007f;
    [ExportGroup("Children")]
    [Export] Polygon2D terrainPoly;
    [Export] Line2D terrainLine;
    [Export] StaticBody2D collisionBody;
    [Export] AnimatedSprite2D finishFlag;
    PackedScene gasCanScene = GD.Load<PackedScene>("res://Scenes/GasCan.tscn");
    public override void _Ready()
    {
        Vector2[] vertices;
        vertices = GenerateTerrainVertices();
        AddGasCans(vertices.ToArray());
        ApplyVerticesToChildren(vertices);
    }

    private Vector2[] GenerateTerrainVertices()
    {
        FastNoiseLite noise = new FastNoiseLite();
        List<Vector2> vertices = new List<Vector2>();
        ConfigureNoise(noise);
        for (int i = 0; i <= totalSliceCount; i++)
        {
            Vector2 point = new Vector2
            {
                X = i * sliceWidth,
                Y = noise.GetNoise1D(i) * yVariation * (1 + difficulty * (float)i / 1000)
            };
            vertices.Add(point);
        }
        vertices.Add(vertices.Last() + new Vector2(200, 0));
        Vector2 enclosingPoint = new Vector2(vertices.Last().X, -distToGround);
        vertices.Add(enclosingPoint);
        vertices.Add(enclosingPoint + new Vector2(2000, 0));
        vertices.Add(enclosingPoint + new Vector2(2000, 2 * distToGround));
        enclosingPoint = new Vector2(vertices.First().X - 2000, distToGround);
        vertices.Add(enclosingPoint);
        vertices.Add(enclosingPoint + new Vector2(0, -2 * distToGround));
        vertices.Add(enclosingPoint + new Vector2(2000, -2 * distToGround));
        return vertices.ToArray();
    }
    private void ApplyVerticesToChildren(Vector2[] vertices)
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
    private void AddGasCans(Vector2[] vertices)
    {
        int distBetweenGasCans = (int)(75 * Math.Log2(difficulty + 2));
        int nodeNum = distBetweenGasCans;
        while (nodeNum < vertices.Length)
        {
            Vector2 pos = vertices[nodeNum];
            GasCan gasCan = gasCanScene.Instantiate<GasCan>();
            gasCan.Position = pos + new Vector2(0, -200);
            AddChild(gasCan);
            nodeNum += (int)(distBetweenGasCans + Math.Log10(nodeNum)); // wraz z dystansem i w zależności od trudności
                                                                    // kanistry będą się pojawiać rzadziej
        }
    }
}
