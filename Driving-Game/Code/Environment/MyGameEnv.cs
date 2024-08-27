using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Godot;

public class MyGameEnv
{
    public int stateSize = 5;
    public int actionSize = 3;
    public int playerNum = 1;
    Level level;
    public MyGameEnv(Level level)
    {
        this.level = level;
        this.level.SpawnPlayers(playerNum);
    }
    public IMLData GetCurrentState(int player = 0)
    {
        if (player >= playerNum)
        {
            return null;
        }
        double[] state = new double[stateSize];
        if (level.players[player].player == null)
        {
            return new BasicMLData(state);
        }
        state[0] = level.players[player].player.playerData.Rotation;
        state[1] = level.players[player].player.playerData.Slope;
        state[2] = level.players[player].player.playerData.DistToGround;
        state[3] = level.players[player].player.playerData.AngularVelocity;
        state[4] = level.players[player].player.playerData.IsTouchingGround ? 1 : 0;
        return new BasicMLData(state);
    }
    public void Reset()
    {
        foreach (var rlapi in level.players)
        {
            rlapi.player.QueueFree();
        }
        level.SpawnPlayers(playerNum);
    }
    public void Step(int actionId, int player = 0)
    {
        InputType input = (InputType)actionId;
        PlayerData currentPlayerState = level.players[player].player.playerData;
        level.players[player].ApplyModelInput(input);
        // level.players[player].player.CalcPlayerData();
    }
}