using Encog.Neural.Flat;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Structure;
using Godot;
using System;

public partial class NetworkDisplayer : Node2D
{
    IContainsFlat network;
    [Export]
    int totalWidth = 500;
    [Export]
    int totalHeight = 300;
    [Export]
    float circleToSpaceRatio = 0.6f;
    float circleRadius, stdLineWidth;
    Vector2[][] nodes;
    Line[] lines;
    FontFile font;
    public void SetNetwork(IContainsFlat network)
    {
        font = ResourceLoader.Load<FontFile>("res://Fonts/AgencyFB-Bold.ttf");
        this.network = network;
        var flat = network.Flat;
        CalcNetworkLayout(flat);
        this.GlobalTranslate(new Vector2(-totalWidth, totalHeight / 2f));
    }
    public void DisplayNetwork(FlatNetwork flat)
    {
        ApplyWeightValues(flat);
        QueueRedraw();
    }
    void CalcNetworkLayout(FlatNetwork flat)
    {
        var layers = flat.LayerCounts.Reverse().ToArray();
        --layers[layers.Length - 1];
        var xOffset = new Vector2((float)totalWidth / (layers.Length - 1), 0);
        var yOffset = new Vector2(0, (float)totalHeight / (layers.Max() - 1));
        var smallestInternodeDist = Math.Min(xOffset.Length(), yOffset.Length());
        circleRadius = smallestInternodeDist * circleToSpaceRatio / 2;
        stdLineWidth = circleRadius / 4;
        nodes = new Vector2[layers.Length][];
        for (int i = 0; i < layers.Length; ++i)
        {
            nodes[i] = new Vector2[layers[i]];
            var layerOffset = yOffset * (layers[i] - 1) / 2;
            for (int j = 0; j < layers[i]; ++j)
            {
                nodes[i][j] = i * xOffset + j * yOffset - layerOffset;
            }
        }
        CalcLinePositions(flat);
    }
    public override void _Draw()
    {
        for (int i = 0; i < lines.Length; ++i)
        {
            DrawLine(lines[i].start, lines[i].end, lines[i].color, lines[i].thickness, antialiased: true);
        }
        for (int i = 0; i < nodes.Length; ++i)
        {
            for (int j = 0; j < nodes[i].Length; ++j)
            {
                DrawCircle(nodes[i][j], circleRadius, new Color(1, 1, 1), antialiased: true);
            }
        }
        // var nameStringOffset = new Vector2(-circleRadius-200, circleRadius / 2);
        // for (int i = 0; i < PlayerData.paramNames.Length; ++i)
        // {
        //     DrawString(font, nodes[0][i]  + nameStringOffset, PlayerData.paramNames[PlayerData.paramNames.Length - 1 - i],
        //      modulate: new Color(0.7f,0.7f,0.7f), fontSize: (int)(circleRadius * 1.5), width: 200,
        //      alignment: HorizontalAlignment.Right);
        // }
    }
    void CalcLinePositions(FlatNetwork flat)
    {
        var numLines = flat.Weights.Length;
        var layers = flat.LayerCounts.Reverse().ToArray();
        lines = new Line[numLines];
        int count = 0;
        for (int i = layers.Length - 2; i >= 0; --i)
        {
            for (int k = 0; k < layers[i + 1] - 1; ++k)
            {
                for (int j = 0; j < layers[i]; ++j)
                {
                    // Console.WriteLine("Layer {0} Node {1} to Layer {2} Node {3} Weight {4} Count {5}", i, j, i + 1, k, flat.GetWeight(i, j, k) - flat.Weights[count], count);
                    // count++;
                    lines[count] = new Line(nodes[i][j], nodes[i + 1][k], stdLineWidth);
                    ++count;
                }
            }
        }
    }
    void ApplyWeightValues(FlatNetwork flat)
    {
        var layers = flat.LayerCounts.Reverse().ToArray();
        int count = 0;
        var layerAvg = new double[layers.Length - 1];
        for (int i = layers.Length - 2; i >= 0; --i)
        {
            for (int k = 0; k < layers[i + 1] - 1; ++k)
            {
                for (int j = 0; j < layers[i]; ++j)
                {
                    layerAvg[i] += Math.Abs(flat.GetWeight(i, j, k));
                }
                layerAvg[i] /= layers[i];
            }
        }
        for (int i = layers.Length - 2; i >= 0; --i)
        {
            for (int k = 0; k < layers[i + 1] - 1; ++k)
            {
                for (int j = 0; j < layers[i]; ++j)
                {
                    lines[count].ApplyValue(flat.GetWeight(i, j, k), layerAvg[i]);
                    ++count;
                }
            }
        }
    }
    class Line
    {
        public Vector2 start, end;
        double min = double.MaxValue, max = double.MinValue;
        public float thickness;
        float stdLineWidth;
        public Color color;
        public Line(Vector2 start, Vector2 end, double stdLineWidth)
        {
            this.start = start;
            this.end = end;
            this.stdLineWidth = (float)stdLineWidth;
        }
        public void ApplyValue(double value, double layerAvg)
        {
            var importance = (float)(Math.Abs(value) / layerAvg / 2);
            if (importance > 1)
            {
                importance = 1;
            }
            thickness = (float)Math.Log(Math.Abs(value) / layerAvg) * stdLineWidth;
            color = value > 0 ? new Color(1, 1-importance, 1-importance) : new Color(1-importance, 1-importance, 1);
        }
    }
}
