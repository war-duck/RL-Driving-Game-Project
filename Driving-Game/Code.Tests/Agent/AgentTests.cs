using Xunit;

namespace Code.Tests;
public class AgentTests
{
    [Fact]
    public void CalcLogProb_CorrectSize_ReturnsLogProbSum()
    {
        double[][] observations = [
            [1, 2, 3],
            [1.5, 2.5, 3.5],
            [2, 3, 4],
            [2.5, 3.5, 4.5]
        ];
        int[] actions = [2, 1, 0 , 2];

        double logProbSum = Agent.CalcLogProb(observations, actions);
        Assert.Equal(-4.63042385778, logProbSum, 1e-10);
    }

    [Fact]
    public void CalcLogProb_WrongSize_ThrowsException()
    {
        double[][] observations = [
            [1, 2, 3],
            [1.5, 2.5, 3.5],
            [2, 3, 4]
        ];

        int[] actions = [2, 1, 0, 1];
        Assert.Throws<Exception>(() => Agent.CalcLogProb(observations, actions));
    }

    [Fact]
    public void CalcDiscountedCumSums_ReturnsDiscountedSums()
    {
        double[] rewards = [1.0, 2.0, 3.0, 4.0, 5.0];
        double gamma = 0.99;
        double[] discountedSums = Agent.CalcDiscountedCumSums(rewards, gamma);
        double[] expected = [14.604476, 13.741895, 11.8605, 8.95, 5];
        for (int i = 0; i < rewards.Length; i++)
        {
            Assert.Equal(expected[i], discountedSums[i], 1e-7);
        }
    }
}