using Godot;

public class RLAPI
{
    public Player player;

    public RLAPI(Player player)
    {
        this.player = player;
    }

    public void ApplyModelInput(InputType input)
    {
        player.SetCurrentInput(input);
    }
    public void KillPlayer()
    {
        player.QueueFree();
    }
    // public int GetActionFromModel(int[] state)
    // {
    //     using (Py.GIL())
    //     {
    //         dynamic rlModel = Py.Import("rl_model");
    //         dynamic action = rlModel.predict_action(state);
    //         return (int)action;
    //     }
    // }

    // public void TrainModel(int[] state, int action, float reward, int[] nextState, bool done)
    // {
    //     using (Py.GIL())
    //     {
    //         dynamic rlModel = Py.Import("rl_model");
    //         rlModel.train_model(state, action, reward, nextState, done);
    //     }
    // }
}
