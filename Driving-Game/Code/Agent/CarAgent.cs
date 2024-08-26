using Encog.App.Quant.Loader.OpenQuant.Data;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Neural.Networks;
using Godot;

public class CarAgent
{
    Agent actor, critic;
    public Buffer buffer;
    public CarAgent() : this(DataLoader.Instance.agentData.inputSize, DataLoader.Instance.agentData.outputSize, DataLoader.Instance.agentData.hiddenLayers) { }

    public CarAgent(int inputSize, int outputSize, int[] hiddenLayers)
    {
        buffer = new Buffer(inputSize);
        actor = new Agent(inputSize, outputSize, hiddenLayers, new ActivationReLU(), new ActivationSoftMax(), buffer);
        critic = new Agent(inputSize, 1, hiddenLayers, new ActivationReLU(), new ActivationLinear(), buffer);
    }
    public (IMLData, InputType) GetAction(IMLData observation)
    {
        IMLData output = actor.model.Compute(observation);
        return (output, (InputType)RouletteWheel.RandomChoice(output));
    }
}