using Encog.App.Quant.Loader.OpenQuant.Data;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Godot;

public class CarAgent
{
    Agent actor, critic;
    public Buffer buffer;
    public CarAgent() : this(DataLoader.Instance.agentData.inputSize, DataLoader.Instance.agentData.outputSize, DataLoader.Instance.agentData.hiddenLayers) { }

    public CarAgent(int inputSize, int outputSize, int[] hiddenLayers)
    {
        buffer = new Buffer();
        actor = new Agent(inputSize, outputSize, hiddenLayers, new ActivationReLU(), new ActivationSoftMax(), buffer);
        critic = new Agent(inputSize, 1, hiddenLayers, new ActivationReLU(), new ActivationLinear(), buffer);
    }
    public CarAgent(BasicNetwork actor, BasicNetwork critic)
    {
        buffer = new Buffer();
        this.actor = new Agent(actor, buffer);
        this.critic = new Agent(critic, buffer);
    }
    public static CarAgent LoadCarAgentFromNewest()
    {
        var files = Directory.GetFiles("Saves/Models/").OrderByDescending(f => new FileInfo(f).CreationTime).ToList();
        var agent = files.Find(f => f.Contains("actor"));
        var critic = files.Find(f => f.Contains("critic"));
        return new CarAgent((BasicNetwork)FileManager.LoadObject(agent, ""), (BasicNetwork)FileManager.LoadObject(critic, ""));
    }
    public (IMLData, InputType) GetAction(IMLData observation)
    {
        IMLData output = actor.model.Compute(observation);
        return (output, (InputType)RouletteWheel.RandomChoice(output));
    }
    public (double, IMLData) LookAhead(IMLData observation)
    {
        return (critic.model.Compute(observation)[0], actor.model.Compute(observation));
    }
    public double GetValue(IMLData observation)
    {
        return critic.model.Compute(observation)[0];
    }
    public void Train()
    {
        double[][] actorGoals, criticGoals;
        for (int i = 0; i < DataLoader.Instance.trainingParams.epochs; i++)
        {
            (actorGoals, criticGoals) = buffer.GetACGoals();
            actor.Train(buffer.GetBuffer().Item1, actorGoals);
            critic.Train(buffer.GetBuffer().Item1, criticGoals);
            buffer.SetStateValues(EvaluateStateValues(buffer.GetBuffer().Item1));
            buffer.CalcAdvantages();
        }
        (actorGoals, criticGoals) = buffer.GetACGoals();
        Console.WriteLine("Actor error: " + actor.model.CalculateError(new BasicMLDataSet(GeneralUtils.IMLDataArrayToDoubleArray(buffer.GetBuffer().Item1), actorGoals)));
        Console.WriteLine("Critic error: " + critic.model.CalculateError(new BasicMLDataSet(GeneralUtils.IMLDataArrayToDoubleArray(buffer.GetBuffer().Item1), criticGoals)));
    }
    public double[] EvaluateStateValues(IMLData[] observations)
    {
        double[] values = new double[observations.Length];
        for (int i = 0; i < observations.Length; i++)
        {
            values[i] = critic.model.Compute(observations[i])[0];
        }
        return values;
    }
    public void SaveNetwork()
    {
        var timestamp = Time.GetDatetimeStringFromSystem();
        FileManager.SaveObject(actor.model, "Saves/Models/", timestamp + "_actor");
        FileManager.SaveObject(critic.model, "Saves/Models/", timestamp + "_critic");
    }
    double[][] GetActorGoals(double[] advantages, double[] logProbs)
    {
        return new double[advantages.Length][];
    }
}