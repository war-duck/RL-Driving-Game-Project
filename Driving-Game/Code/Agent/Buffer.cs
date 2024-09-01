using System.Linq;
using Encog.ML.Data;
using Encog.ML.Data.Buffer;
using Encog.Util;
using Godot;
public class Buffer
{
    double[] rewardBuffer, valueBuffer, advantages, qValues;
    IMLData[] observationBuffer;
    double[][] logProbBuffer;
    double discount, lambda;
    int counter, bufferSize; // counter - ilość elementów w buforze, bufferSize - maksymalna ilość elementów w buforze
    public Buffer()
    {
        TrainingParams trainingParams = DataLoader.Instance.GetTrainingParams();
        discount = trainingParams.discount;
        lambda = trainingParams.lambda;
        bufferSize = trainingParams.batchSize;
        rewardBuffer = new double[bufferSize];
        valueBuffer = new double[bufferSize];
        logProbBuffer = new double[bufferSize][];
        observationBuffer = new IMLData[bufferSize];
        counter = 0;
    }
    public void Add(IMLData observation, double reward, double value, double[] logProb)
    {
        if (counter >= bufferSize)
        {
            throw new OverflowException("Buffer overflow");
        }
        observationBuffer[counter] = observation;
        rewardBuffer[counter] = reward;
        valueBuffer[counter] = value;
        logProbBuffer[counter] = logProb;
        ++counter;
    }
    public void Finish(double lastValue = 0) // 0 jeżeli koniec epizodu (śmierć); inaczej V(S_t)
    {
        qValues = new double[counter];
        double qVal = lastValue;
        for (int t = counter - 1; t >= 0; t--)
        {
            qVal = rewardBuffer[t] + discount * qVal;
            qValues[t] = qVal;
        }
        advantages = qValues;
        for (int t = 0; t < counter; t++)
        {
            advantages[t] -= valueBuffer[t];
        }
    }
    public (IMLData[], double[], double[], double[][], double[]) GetBuffer()
    {
        if (counter != bufferSize)
        {
            return 
            (
                observationBuffer.Take(counter).ToArray(),
                rewardBuffer.Take(counter).ToArray(),
                valueBuffer.Take(counter).ToArray(),
                logProbBuffer.Take(counter).ToArray(),
                advantages.Take(counter).ToArray()
            );
        }
        return (observationBuffer, rewardBuffer, valueBuffer, logProbBuffer, advantages);
    }
    public (double[][], double[][]) GetACGoals()
    {
        double[][] actorGoals = new double[counter][];
        for (int t = 0; t < counter; t++)
        {
            actorGoals[t] = CalcActorGoal(advantages[t], logProbBuffer[t]);
        }
        return (actorGoals, GeneralUtils.To2DArray(qValues));
    }
    public void Reset()
    {
        counter = 0;
    }
    double[] CalcActorGoal(double advantage, double[] logProb)
    {
        double[] actorGoal = new double[logProb.Length];
        for (int i = 0; i < logProb.Length; i++)
        {
            actorGoal[i] = advantage * logProb[i];
        }
        return actorGoal;
    }
}