namespace Code.Tests;

[TestClass]
public class PPOAgentTests
{
    [TestMethod]
    public void CalcLogProb_CorrectSize_ReturnsLogProbSum()
    {
        double[,] observations = new double[,] {
        { 1.0, 2.0, 3.0 },
        { 1.5, 2.5, 3.5 },
        { 2.0, 3.0, 4.0 },
        { 2.5, 3.5, 4.5 }
        };

    int[] actions = new int[] { 2, 1, 0 , 2};

    double logProbSum = PPOAgent.CalcLogProb(observations, actions);
    Assert.AreEqual(-4.63042385778, logProbSum, 1e-10);
    }

    [TestMethod]
    public void CalcLogProb_WrongSize_ThrowsException()
    {
        double[,] observations = new double[,] {
        { 1.0, 2.0, 3.0 },
        { 1.5, 2.5, 3.5 },
        { 2.0, 3.0, 4.0 }
        };

        int[] actions = [2, 1, 0, 1];

        Assert.ThrowsException<Exception>(() => PPOAgent.CalcLogProb(observations, actions));
    }
}