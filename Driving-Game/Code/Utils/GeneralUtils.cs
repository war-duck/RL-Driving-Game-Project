using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Data.Basic;
using Godot;
public static class GeneralUtils
{
    public static double[] ToDoubleArray(IMLData data)
    {
        double[] result = new double[data.Count];
        data.CopyTo(result, 0, data.Count);
        return result;
    }
    public static T[][] To2DArray<T>(T[] data)
    {
        T[][] result = new T[data.Length][];
        for (int i = 0; i < data.Length; ++i)
        {
            result[i] = [data[i]];
        }
        return result;
    }
    public static double[][] IMLDataArrayToDoubleArray(IMLData[] data)
    {
        double[][] result = new double[data.Length][];
        for (int i = 0; i < data.Length; ++i)
        {
            result[i] = ToDoubleArray(data[i]);
        }
        return result;
    }
    static ActivationSoftMax softmax = new ActivationSoftMax();
    public static double[] ToSoftmax(double[] data)
    {
        softmax.ActivationFunction(data, 0, data.Length);
        return data;
    }
    public static double[] GetSoftmaxCopy(double[] data)
    {
        double[] result = new double[data.Length];
        Array.Copy(data, result, data.Length);
        softmax.ActivationFunction(result, 0, result.Length);
        return result;
    }
    public static void ParseCMDLineArgs()
    {
        var args = OS.GetCmdlineArgs();
        foreach (var arg in args)
        {
            if (arg.Contains("activationfunction"))
            {
                switch (arg.Split('=')[1])
                {
                    case "TANH":
                        DataLoader.Instance.agentData.activationFunction = new ActivationTANH();
                        DataLoader.Instance.agentData.activationFunctionName = "TANH";
                        break;
                    case "RELU":
                        DataLoader.Instance.agentData.activationFunction = new ActivationReLU();
                        DataLoader.Instance.agentData.activationFunctionName = "RELU";
                        break;
                    case "SIGMOID":
                        DataLoader.Instance.agentData.activationFunction = new ActivationSigmoid();
                        DataLoader.Instance.agentData.activationFunctionName = "SIGMOID";
                        break;
                    case "SOFTMAX":
                        DataLoader.Instance.agentData.activationFunction = new ActivationSoftMax();
                        DataLoader.Instance.agentData.activationFunctionName = "SOFTMAX";
                        break;
                    default:
                        throw new ArgumentException("Invalid activation function");
                }
            }
            if (arg.Contains("hiddenlayers"))
            {
                DataLoader.Instance.agentData.hiddenLayers = arg.Split('=')[1].Split(',').Select(int.Parse).ToArray();
            }
            if (arg.Contains("batchsize"))
            {
                DataLoader.Instance.trainingParams.batchSize = int.Parse(arg.Split('=')[1]);
            }
            if (arg.Contains("epochs"))
            {
                DataLoader.Instance.trainingParams.epochs = int.Parse(arg.Split('=')[1]);
            }
            if (arg.Contains("discount"))
            {
                DataLoader.Instance.trainingParams.discount = double.Parse(arg.Split('=')[1]);
            }
        }
    }
}