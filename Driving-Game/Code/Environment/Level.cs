using Encog.ML.Data;
using Encog.ML.Data.Buffer;
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
            // (IMLData policyDist, int action) = rlapi.ProcessModelInput(delta);
            var observation = rlapi.player.playerData.ToMLData();
            var (value, policyDist) = rlapi.carAgent.LookAhead(observation);
            var action = RouletteWheel.RandomChoice(policyDist);
            var logProb = Agent.CalcLogProb(policyDist);
            var entropy = Agent.CalcEntropy(policyDist, logProb);
            var (reward, isDead) = rlapi.Step(action, delta);
            try
            {
                rlapi.carAgent.buffer.Add(observation, reward, value, logProb, action);
                if (isDead)
                {
                    throw new OverflowException();
                }
            }
            catch (OverflowException) // buffer full
            {
                rlapi.ProcessBuffer();
                rlapi.carAgent.Train();
                rlapi.carAgent.buffer.Reset();
            }
            if (isDead)
            {
                ResetPlayer(rlapi);
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
			AddChild(playerInstance);
            players.Add(new RLAPI(playerInstance, new CarAgent()));
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
    private void ResetPlayer(RLAPI rlapi)
    {
        rlapi.KillPlayer();
        Player playerInstance = playerScene.Instantiate() as Player;
        AddChild(playerInstance);
        rlapi.player = playerInstance;
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
