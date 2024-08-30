using System;
using Encog.ML.Data;
using Godot;

public class RLAPI
{
    private static double rewardDistance = 10;
    public Player player;
    public CarAgent carAgent;
    private int maxEpisodeLength, currentEpisodeLength = 0;
    private double firstPosition = 0;
    private int lastDistance = 0;
    public RLAPI(Player player, CarAgent agent)
    {
        this.player = player;
        this.carAgent = agent;
        firstPosition = player.playerData.GlobalPositionX;
        maxEpisodeLength = DataLoader.Instance.GetTrainingParams().maxEpisodeLength;
    }

    public void ApplyModelInput(InputType input)
    {
        player.SetCurrentInput(input);
    }
    public (IMLData, int) ProcessModelInput(double delta)
    {
        var (probs, action) = carAgent.GetAction(player.playerData.ToMLData());
        player.SetCurrentInput(action);
        Console.WriteLine("Action: " + action + "\t Observation: " + probs);
        player.MovePlayer(delta);
        ++currentEpisodeLength;
        return (probs, (int)action);
    }
    public void KillPlayer()
    {
        player.QueueFree();
    }
    public (double, bool) GetReward()
    {
        if (player.playerData.HasDied)
        {
            KillPlayer();
            return (-10, true);
        }
        if (currentEpisodeLength >= maxEpisodeLength)
        {
            KillPlayer();
            return (0, true);
        }
        return (GetRecentCheckpoints(), false);
    }
    public int GetRecentCheckpoints()
    {
        int recentCheckpoints = (int)((player.playerData.GlobalPositionX - firstPosition) / rewardDistance - lastDistance);
        lastDistance += recentCheckpoints;
        return recentCheckpoints;
    }
}
