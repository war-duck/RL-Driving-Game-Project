using Encog.ML.Data.Basic;
using Xunit;

public class RouletteWheelTests
{
    [Fact]
    public void RandomChoice_ReturnsValidIndex()
    {
        var probabilities = new BasicMLData(new double[] { 0.2, 0.3, 0.5 });
        var random = new Random(0);

        var result = RouletteWheel.RandomChoice(probabilities, random);
        for (int i = 0; i < 1000; i++)
        {
            result = RouletteWheel.RandomChoice(probabilities, random);
            Assert.InRange(result, 0, probabilities.Count - 1);
        }
    }
}