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
        }
        Console.WriteLine("Actor error: " + actor.model.CalculateError(new BasicMLDataSet(GeneralUtils.IMLDataArrayToDoubleArray(buffer.GetBuffer().Item1), buffer.GetACGoals().Item1)));
        Console.WriteLine("Critic error: " + critic.model.CalculateError(new BasicMLDataSet(GeneralUtils.IMLDataArrayToDoubleArray(buffer.GetBuffer().Item1), buffer.GetACGoals().Item2)));
    }
    double[][] GetActorGoals(double[] advantages, double[] logProbs)
    {
        return new double[advantages.Length][];
    }
}