using Encog.ML.Data;
public class Buffer
{
    double[] rewardBuffer, valueBuffer, advantages, qValues;
    int[] actionBuffer;
    IMLData[] observationBuffer;
    double[][] logProbBuffer;
    double discount, lambda;
    int counter, bufferSize; // counter - ilość elementów w buforze, bufferSize - maksymalna ilość elementów w buforze
    bool isFull;
    double lastValue;
    public int stepCount;
    public Buffer()
    {
        TrainingParams trainingParams = DataLoader.Instance.GetTrainingParams();
        discount = trainingParams.discount;
        lambda = trainingParams.lambda;
        bufferSize = trainingParams.batchSize;
        rewardBuffer = new double[bufferSize];
        valueBuffer = new double[bufferSize];
        logProbBuffer = new double[bufferSize][];
        actionBuffer = new int[bufferSize];
        observationBuffer = new IMLData[bufferSize];
        counter = 0;
    }
    public void Add(IMLData observation, double reward, double value, double[] logProb, int action)
    {
        if (isFull)
        {
            throw new OverflowException("Buffer overflow");
        }
        ++stepCount;
        if (counter == bufferSize - 1)
        {
            isFull = true;
        }
        observationBuffer[counter] = observation;
        rewardBuffer[counter] = reward;
        valueBuffer[counter] = value;
        logProbBuffer[counter] = logProb;
        actionBuffer[counter] = action;
        ++counter;
    }
    public void Finish(double lastValue = 0) // 0 jeżeli koniec epizodu (śmierć); inaczej V(S_t)
    {
        this.lastValue = lastValue;
        CalcAdvantages();
    }
    public (IMLData[], double[], double[], double[][], double[], int[]) GetBuffer()
    {
        if (counter != bufferSize)
        {
            return 
            (
                observationBuffer.Take(counter).ToArray(),
                rewardBuffer.Take(counter).ToArray(),
                valueBuffer.Take(counter).ToArray(),
                logProbBuffer.Take(counter).ToArray(),
                advantages.Take(counter).ToArray(),
                actionBuffer.Take(counter).ToArray()
            );
        }
        return (observationBuffer, rewardBuffer, valueBuffer, logProbBuffer, advantages, actionBuffer);
    }
    public (double[][], double[][]) GetACGoals()
    {
        double[][] actorGoals = new double[counter][];
        for (int t = 0; t < counter; t++)
        {
            // actorGoals[t] = CalcActorGoal(advantages[t], logProbBuffer[t]);
            actorGoals[t] = CalcActorGoal(advantages[t], actionBuffer[t]);
        }
        return (actorGoals, GeneralUtils.To2DArray(qValues));
    }
    public void Reset()
    {
        counter = 0;
        isFull = false;
    }
    public void SetStateValues(double[] stateValues)
    {
        Array.Copy(stateValues, valueBuffer, stateValues.Length);
    }
    double[] CalcActorGoal(double advantage, double[] logProb)
    {
        double[] actorGoal = new double[logProb.Length];
        for (int i = 0; i < logProb.Length; i++)
        {
            actorGoal[i] = advantage * logProb[i];
        }
        return GeneralUtils.ToSoftmax(actorGoal);
    }
    double[] CalcActorGoal(double advantage, int action)
    {
        double[] actorGoal = new double[3];
        actorGoal[action] = advantage;
        return GeneralUtils.ToSoftmax(actorGoal);
    }
    public void CalcAdvantages()
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
}