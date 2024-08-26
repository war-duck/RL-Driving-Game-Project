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
            batchSize = 64,
            epochs = 10,
            policyTrainSteps = 80,
            valueTrainSteps = 80,
            discount = 0.99,
            lambda = 0.95,
            clipRatio = 0.2,
            policyLearningRate = 0.0003,
            valueLearningRate = 0.001,
            targetKLDivergence = 0.01
        };
        agentData = new AgentData
        {
            inputSize = PlayerData.trainingParamsCount,
            outputSize = 3,
            hiddenLayers = new int[] { 13 }
        };
    }
    public TrainingParams GetTrainingParams()
    {
        return trainingParams;
    }
}
public struct TrainingParams
{
    public int batchSize;
    public int epochs;
    public int policyTrainSteps;
    public int valueTrainSteps;
    public double discount;
    public double lambda;
    public double clipRatio;
    public double policyLearningRate;
    public double valueLearningRate;
    public double targetKLDivergence;
}
public struct AgentData
{
    public int inputSize;
    public int outputSize;
    public int[] hiddenLayers;
}