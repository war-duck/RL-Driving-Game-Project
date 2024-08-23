using System.Linq;
public class Buffer
{
    double[,] observationBuffer;
    int[] actionBuffer;
    double[] advantageBuffer, rewardBuffer, returnBuffer, valueBuffer, logProbBuffer;
    double discount, lambda;
    int counter, bufferSize; // start - start obecnej trajektorii w buforze, counter - ilość elementów w buforze, bufferSize - maksymalna ilość elementów w buforze
    public Buffer(int inputSize, int bufferSize, double discount, double lambda)
    {
        observationBuffer = new double[bufferSize,inputSize];
        actionBuffer = new int[bufferSize];
        advantageBuffer = new double[bufferSize];
        rewardBuffer = new double[bufferSize];
        returnBuffer = new double[bufferSize];
        valueBuffer = new double[bufferSize];
        logProbBuffer = new double[bufferSize];
        this.discount = discount;
        this.lambda = lambda;
        this.counter = 0;
        this.bufferSize = bufferSize;
    }
    public void Add(double[] observation, int action, double reward, double value, double logProb)
    {
        if (counter >= bufferSize)
        {
            throw new System.Exception("Buffer overflow");
        }
        for (int i = 0; i < observation.Length; i++)
        {
            observationBuffer[counter, i] = observation[i];
        }
        actionBuffer[counter] = action;
        rewardBuffer[counter] = reward;
        valueBuffer[counter] = value;
        logProbBuffer[counter] = logProb;
        ++counter;
    }
    public void Finish(double lastValue = 0) // 0 jeżeli koniec epizodu (śmierć); inaczej V(S_t)
    {
        // przycinamy bufor, dodajemy ostatnią wartość
        double[] rewards = rewardBuffer.Take(counter).Append(lastValue).ToArray();
        double[] values  = valueBuffer.Take(counter).Append(lastValue).ToArray();
        double[] deltas = new double[counter];
        for (int i = 0; i < counter; i++)
        {
            deltas[i] = rewards[i] + discount * values[i + 1] - values[i];
        }

        // liczymy advantage
        double[] cumsum = PPOAgent.CalcDiscountedCumSums(deltas, discount * lambda);
        for (int i = 0; i < counter; ++i)
        {
            advantageBuffer[i] = cumsum[i];
        }
        // liczymy rewards-to-go
        cumsum = PPOAgent.CalcDiscountedCumSums(rewards.SkipLast(1).ToArray(), discount);
        for (int i = 0; i < counter; ++i)
        {
            returnBuffer[i] = cumsum[i];
        }
    }
    public (double[,], int[], double[], double[], double[]) GetBuffer()
    {
        counter = 0;
        NormalizeAdvantages();
        return (observationBuffer, actionBuffer, advantageBuffer, returnBuffer, logProbBuffer);

    }
    private void NormalizeAdvantages()
    {
        double advantageMean = advantageBuffer.Average();
        double advantageStd = advantageBuffer.Select(x => (x - advantageMean) * (x - advantageMean)).Sum() / (advantageBuffer.Length - 1);
        advantageBuffer = advantageBuffer.Select(x => (x - advantageMean) / advantageStd).ToArray();
    }
}