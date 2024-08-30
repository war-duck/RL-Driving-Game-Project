using System.Linq;
using Encog.ML.Data;
using Encog.ML.Data.Buffer;
public class Buffer
{
    IMLData[] observationBuffer;
    int[] actionBuffer;
    double[] advantageBuffer, rewardBuffer, returnBuffer, valueBuffer, logProbBuffer;
    double discount, lambda;
    int counter, bufferSize; // counter - ilość elementów w buforze, bufferSize - maksymalna ilość elementów w buforze
    public Buffer(int inputSize)
    {
        TrainingParams trainingParams = DataLoader.Instance.GetTrainingParams();
        discount = trainingParams.discount;
        lambda = trainingParams.lambda;
        bufferSize = trainingParams.batchSize;
        observationBuffer = new IMLData[bufferSize];
        actionBuffer = new int[bufferSize];
        advantageBuffer = new double[bufferSize];
        rewardBuffer = new double[bufferSize];
        returnBuffer = new double[bufferSize];
        valueBuffer = new double[bufferSize];
        logProbBuffer = new double[bufferSize];
        counter = 0;
    }
    public void Add(IMLData observation, int action, double reward, double value)
    {
        if (counter >= bufferSize)
        {
            throw new OverflowException("Buffer overflow");
        }
        observationBuffer[counter] = observation;
        actionBuffer[counter] = action;
        rewardBuffer[counter] = reward;
        valueBuffer[counter] = value;
        ++counter;
    }
    public void Finish(double lastValue = 0) // 0 jeżeli koniec epizodu (śmierć); inaczej V(S_t)
    {
        // przycinamy bufor, dodajemy ostatnią wartość
        double[] rewards = rewardBuffer.Take(counter).Append(lastValue).ToArray();
        // double[] values  = valueBuffer.Take(counter).Append(lastValue).ToArray();
        double[] values = new double[counter + 1];
        double[] deltas = new double[counter];
        for (int i = 0; i < counter; i++)
        {
            deltas[i] = rewards[i] + discount * values[i + 1] - values[i];
        }

        // liczymy advantage
        double[] cumsum = Agent.CalcDiscountedCumSums(deltas, discount * lambda);
        for (int i = 0; i < counter; ++i)
        {
            advantageBuffer[i] = cumsum[i];
        }
        // liczymy rewards-to-go
        cumsum = Agent.CalcDiscountedCumSums(rewards.SkipLast(1).ToArray(), discount);
        for (int i = 0; i < counter; ++i)
        {
            returnBuffer[i] = cumsum[i];
        }
    }
    public (IMLData[], int[], double[], double[], double[]) GetBuffer()
    {
        return (observationBuffer, actionBuffer, advantageBuffer, returnBuffer, logProbBuffer);
    }
    public void Reset()
    {
        counter = 0;
    }
    private void NormalizeAdvantages()
    {
        double advantageMean = advantageBuffer.Average();
        double advantageStd = advantageBuffer.Select(x => (x - advantageMean) * (x - advantageMean)).Sum() / (advantageBuffer.Length - 1);
        advantageBuffer = advantageBuffer.Select(x => (x - advantageMean) / advantageStd).ToArray();
    }
}