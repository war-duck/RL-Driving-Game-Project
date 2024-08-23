using Encog.ML.Data;

public class CarAgent : PPOAgent
{
    public CarAgent(int inputSize, int outputSize, int[] hiddenLayers) : base(inputSize, outputSize, hiddenLayers)
    {
    }
    public InputType GetAction(IMLData observation)
    {
        IMLData output = model.Compute(observation);
        return (InputType)RouletteWheel.RandomChoice(output);
    }
}