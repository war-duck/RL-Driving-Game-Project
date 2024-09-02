using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Data.Basic;
public static class GeneralUtils
{
    public static double[] ToDoubleArray(IMLData data)
    {
        double[] result = new double[data.Count];
        data.CopyTo(result, 0, data.Count);
        return result;
    }
    public static T[][] To2DArray<T>(T[] data)
    {
        T[][] result = new T[data.Length][];
        for (int i = 0; i < data.Length; ++i)
        {
            result[i] = [data[i]];
        }
        return result;
    }
    public static double[][] IMLDataArrayToDoubleArray(IMLData[] data)
    {
        double[][] result = new double[data.Length][];
        for (int i = 0; i < data.Length; ++i)
        {
            result[i] = ToDoubleArray(data[i]);
        }
        return result;
    }
    static ActivationSoftMax softmax = new ActivationSoftMax();
    public static double[] ToSoftmax(double[] data)
    {
        softmax.ActivationFunction(data, 0, data.Length);
        return data;
    }
    public static double[] GetSoftmaxCopy(double[] data)
    {
        double[] result = new double[data.Length];
        Array.Copy(data, result, data.Length);
        softmax.ActivationFunction(result, 0, result.Length);
        return result;
    }
}