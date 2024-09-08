using Encog.Engine.Network.Activation;

public class DataLoader
{
    private static DataLoader instance;
    public  TrainingParams trainingParams;
    public AgentData agentData;
    public static DataLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DataLoader();
            }
            return instance;
        }
    }
    public DataLoader()
    {
        trainingParams = new TrainingParams
        {
            batchSize = 1024,
            epochs = 1,
            discount = 0.96,
            maxEpisodeLength = 12_000, // around 3 minutes
            maxTrainingSteps = 1_000_000, // around 4.5 hours of training
            advNormClip = 5
        };
        agentData = new AgentData
        {
            inputSize = PlayerData.trainingParamsCount,
            outputSize = 3,
            hiddenLayers = new int[] { 5 },
            activationFunction = new ActivationTANH(),
            activationFunctionName = "TANH"
        };
    }
    public TrainingParams GetTrainingParams()
    {
        return trainingParams;
    }
    public string GetAgentParamString()
    {
        return "act-" + agentData.activationFunctionName 
            + "-hid-" + string.Join("_", agentData.hiddenLayers) 
            + "-batch-" + trainingParams.batchSize 
            + "-epochs-" + trainingParams.epochs 
            + "-discount-" + trainingParams.discount;
    }
}
public struct TrainingParams
{
    public int batchSize;
    public int epochs;
    public double discount;
    public int maxEpisodeLength;
    public int maxTrainingSteps;
    public double advNormClip;
}
public struct AgentData
{
    public int inputSize;
    public int outputSize;
    public int[] hiddenLayers;
    public IActivationFunction activationFunction;
    public string activationFunctionName;
}