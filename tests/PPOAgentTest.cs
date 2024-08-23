using System;
using 
namespace LogProbTest
{
    class Program
    {

        static void Main(string[] args)
        {
            double[,] observations = new double[,] {
                { 1.0, 2.0, 3.0 },
                { 1.5, 2.5, 3.5 },
                { 2.0, 3.0, 4.0 }
            };

            int[] actions = new int[] { 2, 1, 0 };

            float logProbSum = PPOAgent.CalcLogProb(observations, actions);
            Console.WriteLine("Log Probability Sum: " + logProbSum);
        }
    }
}