using System;
using Encog;
using Encog.Engine.Network.Activation;
using Encog.ML.Model;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Back;
using Godot;
public class PPOAgent
{
    BasicNetwork model;
    public PPOAgent(int inputSize = 5, int outputSize = 3, int[] hiddenLayers = null)
    {
        hiddenLayers ??= new int[] { 5 };
        GenerateModel(inputSize, hiddenLayers, outputSize, new ActivationReLU(), new ActivationSoftMax()); 
        // model.Compile(optimizer: "adam", loss: "categorical_crossentropy");
    }

    private void GenerateModel(int inputSize, int[] hiddenLayerSizes, int outputSize,
                    IActivationFunction activationFunction, IActivationFunction outputActivationFunction)
    {
        var model = new BasicNetwork();
        model.AddLayer(new BasicLayer(null, true, inputSize));
        foreach (var layerSize in hiddenLayerSizes)
        {
            model.AddLayer(new BasicLayer(activationFunction, true, layerSize));
        }
        model.AddLayer(new BasicLayer(outputActivationFunction, true, outputSize));
        model.Structure.FinalizeStructure();
        model.Reset();
    }
    public int StartEpisode()
    {
        //jeżeli błąd mniej niż 0.1 to zwróć 1
        //inaczej zaczynamy nowy epizod
        return 0;
    }
    public static float[] CalcDiscountedCumSums(float[] rewards, float gamma)
    {
        float[] discountedSums = new float[rewards.Length];
        float sum = 0;
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
            throw new System.Exception("Observations and actions must have the same length");
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