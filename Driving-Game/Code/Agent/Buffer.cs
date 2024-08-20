using System.Linq;

public class Buffer
{
    float[,] observationBuffer;
    int[] actionBuffer;
    float[] advantageBuffer, rewardBuffer, returnBuffer, valueBuffer, logProbBuffer;
    float discount, lambda;
    int start, counter, bufferSize; // start - start obecnej trajektorii w buforze, counter - ilość elementów w buforze, bufferSize - maksymalna ilość elementów w buforze
    public Buffer(int inputSize, int bufferSize, float discount, float lambda)
    {
        observationBuffer = new float[bufferSize,inputSize];
        actionBuffer = new int[bufferSize];
        advantageBuffer = new float[bufferSize];
        rewardBuffer = new float[bufferSize];
        returnBuffer = new float[bufferSize];
        valueBuffer = new float[bufferSize];
        logProbBuffer = new float[bufferSize];
        this.discount = discount;
        this.lambda = lambda;
        this.start = 0;
        this.counter = 0;
        this.bufferSize = bufferSize;
    }
    public void Add(float[] observation, int action, float reward, float value, float logProb)
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
    public void Finish(float lastValue = 0) // 0 jeżeli koniec epizodu (śmierć); inaczej V(S_t)
    {
        // przycinamy bufor, dodajemy ostatnią wartość
        float[] rewards = rewardBuffer.Skip(start).Take(counter - start).Append(lastValue).ToArray();
        float[] values  = valueBuffer.Skip(start).Take(counter - start).Append(lastValue).ToArray();
        float[] deltas = new float[counter];
        for (int i = start; i < counter; i++)
        {
            deltas[i] = rewards[i] + discount * values[i + 1] - values[i];
        }

        // liczymy advantage
        float[] cumsum = PPOAgent.CalcDiscountedCumSums(deltas, discount * lambda);
        for (int i = start; i < counter; ++i)
        {
            advantageBuffer[i] = cumsum[i - start];
        }
        // liczymy rewards-to-go
        cumsum = PPOAgent.CalcDiscountedCumSums(rewards.SkipLast(1).ToArray(), discount);
        for (int i = start; i < counter; ++i)
        {
            returnBuffer[i] = cumsum[i - start];
        }
    }
    public (float[,], int[], float[], float[], float[]) GetBuffer()
    {
        counter = start = 0;
        NormalizeAdvantages();
        return (observationBuffer, actionBuffer, advantageBuffer, returnBuffer, logProbBuffer);


    }
    private void NormalizeAdvantages()
    {
        float advantageMean = advantageBuffer.Average();
        float advantageStd = advantageBuffer.Select(x => (x - advantageMean) * (x - advantageMean)).Sum() / (advantageBuffer.Length - 1);
        advantageBuffer = advantageBuffer.Select(x => (x - advantageMean) / advantageStd).ToArray();
    }
}