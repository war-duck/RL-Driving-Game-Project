using System;
using Encog.Engine.Network.Activation;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
public class PPOAgent
{
    public BasicNetwork model;
    public PPOAgent(int inputSize, int outputSize, int[] hiddenLayers)
    {
        GenerateModel(inputSize, hiddenLayers, outputSize, new ActivationReLU(), new ActivationSoftMax());
    }

    private void GenerateModel(int inputSize, int[] hiddenLayerSizes, int outputSize,
                    IActivationFunction activationFunction, IActivationFunction outputActivationFunction)
    {
        model = new BasicNetwork();
        model.AddLayer(new BasicLayer(null, true, inputSize));
        foreach (var layerSize in hiddenLayerSizes)
        {
            model.AddLayer(new BasicLayer(activationFunction, true, layerSize));
        }
        model.AddLayer(new BasicLayer(outputActivationFunction, true, outputSize));
        model.Structure.FinalizeStructure();
        model.Reset();
    }
    public static double[] CalcDiscountedCumSums(double[] rewards, double gamma)
    {
        double[] discountedSums = new double[rewards.Length];
        double sum = 0;
        for (int i = rewards.Length - 1; i >= 0; i--)
        {
            sum = rewards[i] + gamma * sum;
            discountedSums[i] = sum;
        }
        return discountedSums;
    }
    public static double CalcLogProb(double[,] observations, int[] actions)
    {
        if (observations.GetLength(0) != actions.Length)
        {
            throw new Exception("Observations and actions must have the same length");
        }
        IActivationFunction activationFunction = new ActivationSoftMax();
        double logProbSum = 0f;
        int width = observations.GetLength(1);
        double[] row = new double[width];
        for (int i = 0; i < actions.Length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                row[j] = observations[i, j];
            }
            activationFunction.ActivationFunction(row, 0, width);
            logProbSum += Math.Log(row[actions[i]]);
        }
        return logProbSum;
    }
}