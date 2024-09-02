using Encog.ML.Data;
using Encog.ML.Data.Basic;

public class PlayerData
{
    public double GlobalPositionX { get; set; }
    public double Rotation { get; set; }
    public double Slope { get; set; }
    public double DistToGround { get; set; }
    public double AngularVelocity { get; set; }
    public bool IsTouchingGround { get; set; }
    public bool HasDied { get; set; }
    public double Speed { get; set; }
    public static int trainingParamsCount = 6;
    public string Print()
    {
        return $"GlobalPositionX: {GlobalPositionX.ToString("F3")}," +
            $"\t Rotation: {Rotation.ToString("F3")}," +
            $"\t Slope: {Slope.ToString("F3")}," +
            $"\t DistToGround: {DistToGround.ToString("F3")}," +
            $"\t AngularVelocity: {AngularVelocity.ToString("F3")}," +
            $"\t IsTouchingGround: {IsTouchingGround}," +
            $"\t Speed: {Speed.ToString("F3")}";
    }
    public IMLData ToMLData()
    {
        double[] data = new double[trainingParamsCount];
        data[0] = Rotation;
        data[1] = Slope;
        data[2] = DistToGround;
        data[3] = AngularVelocity;
        data[4] = IsTouchingGround ? 1 : 0;
        data[5] = Speed;
        return new BasicMLData(data);
    }
}