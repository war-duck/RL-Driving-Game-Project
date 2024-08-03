using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Terrain : Node
{
    [ExportGroup("Terrain Generation Parameters")]
    [Export] int yVariation = 500;
    [Export] int sliceWidth = 20;
    [Export] int totalSliceCount = 1000;
    [Export] int noiseSeed = 0;
    [Export] int distToGround = 5000;
    [Export(PropertyHint.Range, "0,5,1")] int difficulty = 1;
    [Export(PropertyHint.Range, "-2,2")] float layerGain = 0.5f;
    [ExportGroup("Children")]
    [Export] Polygon2D terrainPoly;
    [Export] Line2D terrainLine;
    [Export] StaticBody2D collisionBody;
    
    public override void _Ready()
    {
        Vector2[] vertices;
        vertices = GenerateTerrainVertices();
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
                Y = noise.GetNoise1D(i) * yVariation * (1 + difficulty * i / totalSliceCount)
            };
            vertices.Add(point);
        }
        Vector2 enclosingPoint = new Vector2(vertices.Last().X, -distToGround);
        vertices.Add(enclosingPoint);
        vertices.Add(enclosingPoint + new Vector2(500, 0));
        vertices.Add(enclosingPoint + new Vector2(500, 2 * distToGround));
        enclosingPoint = new Vector2(vertices.First().X - 500, distToGround);
        vertices.Add(enclosingPoint);
        vertices.Add(enclosingPoint + new Vector2(0, -2 * distToGround));
        vertices.Add(enclosingPoint + new Vector2(500, -2 * distToGround));
        return vertices.ToArray();
    }
    private void ApplyVerticesToChildren(Vector2[] vertices)
    {
        terrainPoly.Set(Polygon2D.PropertyName.Polygon, vertices);
        terrainLine.Set(Line2D.PropertyName.Points, vertices.Take(totalSliceCount).ToArray());
        CollisionPolygon2D collisionPoly = new CollisionPolygon2D();
        collisionPoly.Set(CollisionPolygon2D.PropertyName.Polygon, vertices.ToArray());
        collisionBody.AddChild(collisionPoly);
    }
    private void ConfigureNoise(FastNoiseLite noise)
    {
        noise.Seed = noiseSeed;
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        noise.FractalGain = layerGain;
    }
}
