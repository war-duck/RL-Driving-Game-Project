using Encog.ML.Data;
using Encog.ML.Genetic.Genome;
using Godot;
using System.Collections.Generic;
using System.Windows.Markup;

public partial class Level : Node2D
{
	private PackedScene playerScene = GD.Load<PackedScene>("res://Scenes/Player.tscn");
    private PackedScene rayScene = GD.Load<PackedScene>("res://Scenes/Ray.tscn");
    public List<RLAPI> players = new List<RLAPI>();
	private Camera2D camera;
    public override void _Ready()
    {
		camera = GetNode<Camera2D>("Camera");
        SpawnPlayers(1);
    }

    public override void _Process(double delta)
    {
        camera.GlobalPosition = GetBestPlayerPosition();
        foreach (var rlapi in players)
        {
            // InputType randomInput = (InputType)(GD.Randi() % 3);
            (IMLData observation, int action) = rlapi.ProcessModelInput(delta);
            var (reward, isDead) = rlapi.GetReward();
            var value = rlapi.carAgent.GetValue(observation);
            if (isDead)
            {
                ProcessBuffer(rlapi, observation, action, reward, value);
            }
            try
            {
                rlapi.carAgent.buffer.Add(observation, action, reward, value);
            }
            catch (System.Exception)
            {
                ProcessBuffer(rlapi, observation, action, reward, value);
            }
        }
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
            players.Add(new RLAPI(playerInstance, new CarAgent()));
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
    private void ProcessBuffer(RLAPI rlapi, IMLData observation, int action, double reward, double value)
    {
        rlapi.carAgent.buffer.Finish(value);
        rlapi.carAgent.buffer.Reset();
        rlapi.carAgent.Train();
        rlapi.carAgent.buffer.Add(observation, action, reward, value);
        rlapi.player.QueueFree();
    }
    // private InputType getPlayerInput()
    // {
    //     if (Input.IsActionPressed("ui_right")) // manual steering
    //     {
    //         return InputType.Accelerate;
    //     }
    //     if (Input.IsActionPressed("ui_left"))
    //     {
    //         return InputType.Brake;
    //     }
    //     else
    //     {
    //         return InputType.None;
    //     }
    // }
}
