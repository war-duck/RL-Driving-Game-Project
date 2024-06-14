using Godot;
using System;
using System.Collections.Generic;

public partial class level_1 : Node2D
{
	private PackedScene playerScene = GD.Load<PackedScene>("res://Scenes/Player.tscn");
    private PackedScene rayScene = GD.Load<PackedScene>("res://Scenes/Ray.tscn");
    private List<RLAPI> players = new List<RLAPI>();
	private Camera2D camera;
    public override void _Ready()
    {
		camera = GetNode<Camera2D>("Camera");
        SpawnPlayers(5);
    }

    public override void _Process(double delta)
    {
        foreach (var rlapi in players)
        {
            InputType randomInput = (InputType)(GD.Randi() % 3);
            rlapi.ApplyModelInput(randomInput);
        }
        camera.GlobalPosition = GetBestPlayerPosition();
    }
    public override void _ExitTree()
    {
		foreach (var rlapi in players)
		{
			rlapi.KillPlayer();
		}
        base._ExitTree();
    }
    public void SpawnPlayers(int numberOfPlayers)
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
			Player playerInstance = playerScene.Instantiate() as Player;
            players.Add(new RLAPI(playerInstance));
			AddChild(playerInstance);
        }
    }
	private Vector2 GetBestPlayerPosition()
	{
		Vector2 bestDist = new Vector2(float.NegativeInfinity, 0);
		foreach (var rlapi in players)
		{
			if (rlapi.player.GlobalPosition.X > bestDist.X)
			{
				bestDist = rlapi.player.Position;
			}
		}
		return bestDist;
	}
}
