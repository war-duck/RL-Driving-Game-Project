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
    public static int trainingParamsCount = 5;
    public string Print()
    {
        return $" GlobalPositionX: {GlobalPositionX},\t Rotation: {Rotation},\t Slope: {Slope},\t DistToGround: {DistToGround},\t AngularVelocity: {AngularVelocity.ToString("F3")},\t IsTouchingGround: {IsTouchingGround},\t HasDied: {HasDied}";
    }
    public IMLData ToMLData()
    {
        double[] data = new double[trainingParamsCount];
        data[0] = Rotation;
        data[1] = Slope;
        data[2] = DistToGround;
        data[3] = AngularVelocity;
        data[4] = IsTouchingGround ? 1 : 0;
        return new BasicMLData(data);
    }
}