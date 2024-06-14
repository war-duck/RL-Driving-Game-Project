public class PlayerData
{
    public int GlobalPositionX { get; set; }
    public int Rotation { get; set; }
    public int Slope { get; set; }
    public int DistToGround { get; set; }
    public double AngularVelocity { get; set; }
    public bool IsTouchingGround { get; set; }
    public bool HasDied { get; set; }
    public override string ToString()
    {
        return $" GlobalPositionX: {GlobalPositionX},\n Rotation: {Rotation},\n Slope: {Slope},\n DistToGround: {DistToGround},\n AngularVelocity: {AngularVelocity.ToString("F3")},\n IsTouchingGround: {IsTouchingGround},\n HasDied: {HasDied}";
    }
}