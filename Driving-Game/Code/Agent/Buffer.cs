using Encog.ML.Data;
public class Buffer
{
    double[] rewardBuffer, valueBuffer, advantages, qValues;
    int[] actionBuffer;
    IMLData[] observationBuffer, probBuffer;
    double discount;
    int counter, bufferSize; // counter - ilość elementów w buforze, bufferSize - maksymalna ilość elementów w buforze
    bool isFull;
    double lastValue;
    public int stepCount;
    public Buffer()
    {
        TrainingParams trainingParams = DataLoader.Instance.GetTrainingParams();
        discount = trainingParams.discount;
        bufferSize = trainingParams.batchSize;
        rewardBuffer = new double[bufferSize];
        valueBuffer = new double[bufferSize];
        actionBuffer = new int[bufferSize];
        observationBuffer = new IMLData[bufferSize];
        probBuffer = new IMLData[bufferSize];
        counter = 0;
    }
    public void Add(IMLData observation, double reward, double value, IMLData prob, int action)
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
        probBuffer[counter] = prob;
        actionBuffer[counter] = action;
        ++counter;
    }
    public void Finish(double lastValue = 0) // 0 jeżeli koniec epizodu (śmierć); inaczej V(S_t)
    {
        this.lastValue = lastValue;
        CalcAdvantages();
    }
    public (IMLData[], double[], double[], IMLData[], double[], int[]) GetBuffer()
    {
        if (counter != bufferSize)
        {
            return 
            (
                observationBuffer.Take(counter).ToArray(),
                rewardBuffer.Take(counter).ToArray(),
                valueBuffer.Take(counter).ToArray(),
                probBuffer.Take(counter).ToArray(),
                advantages.Take(counter).ToArray(),
                actionBuffer.Take(counter).ToArray()
            );
        }
        return (observationBuffer, rewardBuffer, valueBuffer, probBuffer, advantages, actionBuffer);
    }
    public (double[][], double[][]) GetACGoals()
    {
        double[][] actorGoals = new double[counter][];
        for (int t = 0; t < counter; t++)
        {
            // actorGoals[t] = CalcActorGoal(advantages[t], logProbBuffer[t]);
            // actorGoals[t] = CalcActorGoal(advantages[t], actionBuffer[t]);
            actorGoals[t] = CalcActorGoal(advantages[t], probBuffer[t], actionBuffer[t]);
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
    // double[] CalcActorGoal(double advantage, double[] logProb)
    // {
    //     double[] actorGoal = new double[logProb.Length];
    //     for (int i = 0; i < logProb.Length; i++)
    //     {
    //         actorGoal[i] = advantage * logProb[i];
    //     }
    //     return GeneralUtils.ToSoftmax(actorGoal);
    // }
    // double[] CalcActorGoal(double advantage, int action)
    // {
    //     double[] actorGoal = new double[3];
    //     actorGoal[action] = advantage;
    //     return GeneralUtils.ToSoftmax(actorGoal);
    // }
    double[] CalcActorGoal(double advantage, IMLData prob, int action)
    {
        double[] actorGoal = new double[prob.Count];
        if (advantage < 0)
        {
            for (int i = 0; i < prob.Count; i++)
            {
                actorGoal[i] = prob[i] * (1 + advantage);
                if (i != action)
                {
                    actorGoal[i] -= advantage;
                }
            }
        }
        else
        {
            for (int i = 0; i < prob.Count; i++)
            {
                actorGoal[i] = prob[i] * (1 - advantage);
                if (i == action)
                {
                    actorGoal[i] += advantage;
                }
            }
        }
        return actorGoal;
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
        // normalize advantages to [-1, 1]
        double maxAdv = Math.Max(advantages.Max(),  DataLoader.Instance.GetTrainingParams().advNormClip);
        double minAdv = Math.Min(advantages.Min(), -DataLoader.Instance.GetTrainingParams().advNormClip);
        for (int t = 0; t < counter; t++)
        {
            advantages[t] = (advantages[t] - minAdv) / (maxAdv - minAdv);
            advantages[t] = 2 * advantages[t] - 1;
        }
    }
}