using System;
using Encog.App.Quant.Loader.OpenQuant.Data;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks.Training.Propagation.Resilient;
public class Agent
{
    public BasicNetwork model;
    Buffer buffer;
    public int batchSize, epochs, policyTrainSteps, valueTrainSteps;
    public double discount, lambda, clipRatio, policyLearningRate, valueLearningRate, targetKLDivergence;
    public Agent(int inputSize, int outputSize, int[] hiddenLayers, Buffer buffer) : this(inputSize, outputSize, hiddenLayers, new ActivationReLU(), new ActivationSoftMax(), buffer) { }

    public Agent(int inputSize, int outputSize, int[] hiddenLayers, IActivationFunction activationFunction, IActivationFunction outputActivationFunction, Buffer buffer)
    {
        GetHyperparameters();
        GenerateModel(inputSize, hiddenLayers, outputSize, activationFunction, outputActivationFunction);
        this.buffer = buffer;
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
    public static double CalcLogProb(double[][] observations, int[] actions)
    {
        if (observations.Length != actions.Length)
        {
            throw new Exception("Observations and actions must have the same length");
        }
        IActivationFunction activationFunction = new ActivationSoftMax();
        double logProbSum = 0f;
        for (int i = 0; i < actions.Length; i++)
        {
            activationFunction.ActivationFunction(observations[i], 0, observations[i].Length);
            logProbSum += Math.Log(observations[i][actions[i]]);
        }
        return logProbSum;
    }
    public IMLData[] Predict(IMLData[] observations)
    {
        var actionProbabilities = new IMLData[observations.Length];
        for (int i = 0; i < observations.Length; i++)
        {
            actionProbabilities[i] = model.Compute(observations[i]);
        }
        return actionProbabilities;
    }
    public void Train(double[][] observations, double[][] targets)
    {
        
        Propagation train = new ResilientPropagation(model, new BasicMLDataSet(observations, targets));
        for (int i = 0; i < epochs; i++)
        {
            train.Iteration();
        }
        Console.WriteLine("Error: " + train.Error);
    }
    void GetHyperparameters()
    {
        TrainingParams trainingParams = DataLoader.Instance.GetTrainingParams();
        batchSize = trainingParams.batchSize;
        epochs = trainingParams.epochs;
        policyTrainSteps = trainingParams.policyTrainSteps;
        valueTrainSteps = trainingParams.valueTrainSteps;
        discount = trainingParams.discount;
        lambda = trainingParams.lambda;
        clipRatio = trainingParams.clipRatio;
        policyLearningRate = trainingParams.policyLearningRate;
        valueLearningRate = trainingParams.valueLearningRate;
        targetKLDivergence = trainingParams.targetKLDivergence;
    }
}