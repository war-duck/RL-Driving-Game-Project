using Encog.ML.Data.Basic;

[TestClass]
public class RouletteWheelTests
{
    [TestMethod]
    public void RandomChoice_ReturnsValidIndex()
    {
        var probabilities = new BasicMLData(new double[] { 0.2, 0.3, 0.5 });
        var random = new Random(0);

        var result = RouletteWheel.RandomChoice(probabilities, random);
        for (int i = 0; i < 1000; i++)
        {
            result = RouletteWheel.RandomChoice(probabilities, random);
            Assert.AreEqual(result, (double)(probabilities.Count - 1) / 2, (double)(probabilities.Count - 1) / 2);
        }
    }
}