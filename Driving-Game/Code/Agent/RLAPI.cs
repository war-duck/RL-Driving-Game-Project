using System;
using Encog.ML.Data;
using Godot;

public class RLAPI
{
    private static double rewardDistance = 10;
    public Player player;
    public CarAgent carAgent;
    int maxEpisodeLength;
    public int currentEpisodeLength = 0;
    private double firstPosition = 0;
    private int lastDistance = 0;
    private int maxDistance;
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
        Console.WriteLine("Action: " + action + "\t Action probs: " + probs);
        player.MovePlayer(delta);
        ++currentEpisodeLength;
        return (probs, (int)action);
    }
    public (double, bool) Step(int action, double delta)
    {
        player.SetCurrentInput((InputType)action);
        player.MovePlayer(delta);
        ++currentEpisodeLength;
        if (player.playerData.GlobalPositionX > maxDistance)
        {
            maxDistance = (int)player.playerData.GlobalPositionX;
        }
        return GetReward();
    }
    public void KillPlayer()
    {
        player.QueueFree();
        currentEpisodeLength = 0;
        lastDistance = 0;
        FileManager.SaveLine(String.Join(",", Time.GetDatetimeStringFromSystem(), maxDistance), name: DataLoader.Instance.GetAgentParamString() + "episode_log");
        maxDistance = 0;
    }
    public (double, bool) GetReward()
    {
        if (player.playerData.HasDied)
        {
            return (-3, true);
        }
        if (currentEpisodeLength >= maxEpisodeLength)
        {
            return (0, true);
        }
        return (GetRecentCheckpoints(), false);
    }
    public IMLData GetObservation()
    {
        return player.playerData.ToMLData();
    }
    public void ProcessBuffer()
    {
        carAgent.buffer.Finish(carAgent.GetValue(GetObservation()));
    }
    int GetRecentCheckpoints()
    {
        int recentCheckpoints = (int)((player.playerData.GlobalPositionX - firstPosition) / rewardDistance - lastDistance);
        lastDistance += recentCheckpoints;
        return recentCheckpoints;
    }
}
