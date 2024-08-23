using System;
using System.Linq;
using Encog.ML.Data;
using Encog.ML.Data.Basic;

public class RouletteWheel
{
    public static int RandomChoice(IMLData probabilities)
    {
        return RandomChoice(probabilities, new Random());
    }
    public static int RandomChoice(IMLData probabilities, Random random)
    {
        double randomValue = random.NextDouble();
        double sum = 0;
        for (int i = 0; i < probabilities.Count; i++)
        {
            sum += probabilities[i];
            if (randomValue < sum)
            {
                return i;
            }
        }
        return probabilities.Count - 1;
    }
}