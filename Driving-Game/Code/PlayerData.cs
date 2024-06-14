public class PlayerData
{
    public int GlobalPositionX { get; set; }
    public int Rotation { get; set; }
    public int Slope { get; set; }
    public int DistToGround { get; set; }
    public double AngularVelocity { get; set; }
    public bool IsTouchingGround { get; set; }
    public bool HasDied { get; set; }
    public string Print()
    {
        return $" GlobalPositionX: {GlobalPositionX},\t Rotation: {Rotation},\t Slope: {Slope},\t DistToGround: {DistToGround},\t AngularVelocity: {AngularVelocity.ToString("F3")},\t IsTouchingGround: {IsTouchingGround},\t HasDied: {HasDied}";
    }
}